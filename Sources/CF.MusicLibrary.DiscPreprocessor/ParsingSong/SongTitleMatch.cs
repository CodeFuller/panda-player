using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace CF.MusicLibrary.DiscPreprocessor.ParsingSong
{
	internal class SongTitleMatch
	{
		public bool Success { get; }

		public string SongTitle { get; }

		private SongTitleMatch()
		{
			Success = false;
		}

		public SongTitleMatch(string rawTitle, string payload)
		{
			Success = true;

			var adjustedTitle = AdjustTitle(rawTitle);
			SongTitle = String.IsNullOrEmpty(payload)
				? CapitalizeTitle(adjustedTitle)
				: Invariant($"{CapitalizeTitle(adjustedTitle)} {payload}");
		}

		public SongTitleMatch(string rawTitle)
			: this(rawTitle, String.Empty)
		{
		}

		public static SongTitleMatch FailedMatch => new SongTitleMatch();

		private static string AdjustTitle(string title)
		{
			return title.Replace('’', '\'');
		}

		private static string CapitalizeTitle(string rawTitle)
		{
			if (TextHasCyrillic(rawTitle))
			{
				return rawTitle;
			}

			// CultureInfo.CurrentCulture.TextInfo.ToTitleCase() doesn't work good for titles like 'RockStar',
			// where middle letters should be in upper case. That's why we reinvent these wheels.
			// var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle);
			var title = ToTitleCase(rawTitle);

			// Handling numeric ordinals, 1st, 2nd, 12th
			return Regex.Replace(title, "(1st)|(2nd)|(3rd)|([0-9]th)",
				m => m.Captures[0].Value.ToLower(CultureInfo.CurrentCulture), RegexOptions.IgnoreCase);
		}

		private static bool TextHasCyrillic(string text)
		{
			return Regex.IsMatch(text, @"\p{IsCyrillic}");
		}

		private static string ToTitleCase(string title)
		{
			return String.Join(String.Empty, SplitTitle(title).Select(CapitalizeFirstLetter));
		}

		private static IEnumerable<string> SplitTitle(string title)
		{
			var regex = new Regex("(\\S+\\s*)");
			MatchCollection matches = regex.Matches(title);
			for (var i = 0; i < matches.Count; ++i)
			{
				yield return matches[i].Value;
			}
		}

		private static string CapitalizeFirstLetter(string str)
		{
			if (String.IsNullOrEmpty(str))
			{
				return str;
			}

			return str.Length > 1 ? Char.ToUpper(str[0], CultureInfo.InvariantCulture) + str.Substring(1) : str.ToUpper(CultureInfo.InvariantCulture);
		}
	}
}
