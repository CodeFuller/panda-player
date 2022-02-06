using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PandaPlayer.DiscAdder.ParsingSong
{
	internal class SongTitlePattern
	{
		private readonly string pattern;
		private readonly Regex regex;

		public string Description { get; init; }

		public string Source { get; init; }

		public string Pattern
		{
			get => pattern;
			init
			{
				pattern = value;
				regex = new Regex(pattern, RegexOptions.Compiled);
			}
		}

		public IReadOnlyCollection<SongParsingTestCase> TestCases { get; init; }

		public SongTitleMatch Match(string rawTitle, Func<string, string> parsePayload)
		{
			var match = regex.Match(rawTitle);
			if (match.Success)
			{
				var matchedTitle = match.Groups[1].Value;
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
