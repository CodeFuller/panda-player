using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingSong
{
	internal class SongTitlePattern
	{
		private string pattern;
		private Regex regex;

		public string Description { get; set; }

		public string Source { get; set; }

		public string Pattern
		{
			get => pattern;
			set
			{
				pattern = value;
				regex = new Regex(pattern, RegexOptions.Compiled);
			}
		}

		public Collection<SongParsingTest> Tests { get; set; }

		public SongTitleMatch Match(string rawTitle, Func<string, string> parsePayload)
		{
			var match = regex.Match(rawTitle);
			if (match.Success)
			{
				string matchedTitle = match.Groups[1].Value;
				return match.Groups.Count > 2 && !String.IsNullOrEmpty(match.Groups[2].Value)
					? new SongTitleMatch(matchedTitle, parsePayload(match.Groups[2].Value))
					: new SongTitleMatch(matchedTitle);
			}
			else
			{
				return SongTitleMatch.FailedMatch;
			}
		}
	}
}
