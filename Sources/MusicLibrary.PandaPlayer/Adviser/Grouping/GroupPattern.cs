using System;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace MusicLibrary.PandaPlayer.Adviser.Grouping
{
	public class GroupPattern
	{
		private Regex regex;

		public string Pattern
		{
			get => regex?.ToString();

			set
			{
				try
				{
					regex = new Regex(value, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				}
				catch (ArgumentException)
				{
					throw new InvalidOperationException($"Failed to parse grouping pattern '{value}'");
				}
			}
		}

		public string GroupId { get; set; }

		public virtual bool Matches(Uri discUri, out string groupId)
		{
			var match = regex.Match(discUri.ToString());
			if (match.Success)
			{
				groupId = BuildGroupId(match);
				return true;
			}

			groupId = String.Empty;
			return false;
		}

		private string BuildGroupId(Match match)
		{
			var groupId = GroupId;
			for (var i = 1; i < match.Groups.Count; ++i)
			{
				groupId = groupId.Replace(Invariant($"${i}"), match.Groups[i].Value, StringComparison.Ordinal);
			}

			return groupId;
		}
	}
}
