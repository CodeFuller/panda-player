using System;
using System.Globalization;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	internal class SongTitleMatch
	{
		public bool Success { get; }

		public string SongTitle { get; }

		public static string CapitalizeTitle(string rawTitle)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle);
		}

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
	}
}
