using System;
using System.Globalization;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	internal class SongTitleMatch
	{
		public bool Success { get; set; }

		public string RawTitle { get; set; }

		public string Payload { get; set; }

		public string SongTitle => String.IsNullOrEmpty(Payload) ? CapitalizeTitle(RawTitle) : FormattableString.Invariant($"{CapitalizeTitle(RawTitle)} {Payload}");

		public static string CapitalizeTitle(string rawTitle)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rawTitle);
		}
	}
}
