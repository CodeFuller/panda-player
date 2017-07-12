using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	internal class SongTitleMatch
	{
		public bool Success { get; }

		public string SongTitle { get; }

		/// <summary>
		/// Constructs not-matched SongTitleMatch.
		/// </summary>
		private SongTitleMatch()
		{
			Success = false;
		}

		/// <summary>
		/// Constructs matched SongTitleMatch with specified title and payload.
		/// </summary>
		public SongTitleMatch(string rawTitle, string payload)
		{
			Success = true;

			var adjustedTitle = AdjustTitle(rawTitle);
			SongTitle = String.IsNullOrEmpty(payload)
				? CapitalizeTitle(adjustedTitle)
				: FormattableString.Invariant($"{CapitalizeTitle(adjustedTitle)} {payload}");
		}

		/// <summary>
		/// Constructs matched SongTitleMatch with specified title and empty payload.
		/// </summary>
		public SongTitleMatch(string rawTitle) :
			this(rawTitle, String.Empty)
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

			var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle);
			//	Handling numeric ordinals, 1st, 2nd, 12th
			return Regex.Replace(title, "(1st)|(2nd)|(3rd)|([0-9]th)",
				m => m.Captures[0].Value.ToLower(CultureInfo.CurrentCulture), RegexOptions.IgnoreCase);
		}

		private static bool TextHasCyrillic(string text)
		{
			return Regex.IsMatch(text, @"\p{IsCyrillic}");
		}
	}
}
