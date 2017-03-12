using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	internal class SongTitlePattern
	{
		private string pattern;
		private Regex regex;

		public string Description { get; set; }

		public string Source { get; set; }

		public string Pattern
		{
			get
			{
				return pattern;
			}
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
			SongTitleMatch result = new SongTitleMatch();
			if (match.Success)
			{
				result.Success = true;
				result.RawTitle = match.Groups[1].Value;
				if (match.Groups.Count > 2 && !String.IsNullOrEmpty(match.Groups[2].Value))
				{
					result.Payload = parsePayload(match.Groups[2].Value);
				}
			}
			else
			{
				result.Success = false;
			}

			return result;
		}
	}
}
