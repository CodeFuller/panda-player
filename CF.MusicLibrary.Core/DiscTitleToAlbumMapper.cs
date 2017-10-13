using System.Text.RegularExpressions;

namespace CF.MusicLibrary.Core
{
	public static class DiscTitleToAlbumMapper
	{
		private static readonly Regex CDRegex = new Regex(@"^(.+) \(CD ?\d+\)$", RegexOptions.Compiled);
		private static readonly Regex SingleRegex = new Regex(@"^(.+) \((?:(?:Single)|(?:EP))\)$", RegexOptions.Compiled);
		private static readonly Regex DemoRegex = new Regex(@"^(.+) \(Demo\)$", RegexOptions.Compiled);
		private static readonly Regex LiveRegex = new Regex(@"^(.+) \(Live\)$", RegexOptions.Compiled);
		private static readonly Regex BonusRegex = new Regex(@"^(.+) \(Bonus(?: CD)?\)$", RegexOptions.Compiled);
		private static readonly Regex CompilationRegex = new Regex(@"^(.+) \(Compilation\)$", RegexOptions.Compiled);
		private static readonly Regex BSidesRegex = new Regex(@"^(.+) \(B-Sides\)$", RegexOptions.Compiled);
		private static readonly Regex RemixesRegex = new Regex(@"^(.+) \(Remixes\)$", RegexOptions.Compiled);
		private static readonly Regex RussianCompilationRegex = new Regex(@"^(.+) \(Сборник\)$", RegexOptions.Compiled);
		private static readonly Regex TributeRegex = new Regex(@"^(.+) \(Трибьют\)$", RegexOptions.Compiled);
		private static readonly Regex SoundtrackRegex = new Regex(@"^(.+) \(Soundtrack\)$", RegexOptions.Compiled);

		public static string GetAlbumTitleFromDiscTitle(string discTitle)
		{
			if (discTitle == null)
			{
				return null;
			}

			if (discTitle == "Rarities")
			{
				return null;
			}

			var albumTitle = discTitle;
			albumTitle = ProcessTitleRegex(albumTitle, CDRegex);
			albumTitle = ProcessTitleRegex(albumTitle, SingleRegex);
			albumTitle = ProcessTitleRegex(albumTitle, DemoRegex);
			albumTitle = ProcessTitleRegex(albumTitle, LiveRegex);
			albumTitle = ProcessTitleRegex(albumTitle, BonusRegex);
			albumTitle = ProcessTitleRegex(albumTitle, CompilationRegex);
			albumTitle = ProcessTitleRegex(albumTitle, BSidesRegex);
			albumTitle = ProcessTitleRegex(albumTitle, RemixesRegex);
			albumTitle = ProcessTitleRegex(albumTitle, RussianCompilationRegex);
			albumTitle = ProcessTitleRegex(albumTitle, TributeRegex);
			albumTitle = ProcessTitleRegex(albumTitle, SoundtrackRegex);

			return albumTitle;
		}

		public static bool AlbumTitleIsSuspicious(string albumTitle)
		{
			return GetAlbumTitleFromDiscTitle(albumTitle) != albumTitle;
		}

		private static string ProcessTitleRegex(string title, Regex regex)
		{
			var match = regex.Match(title);
			return match.Success ? match.Groups[1].Value : title;
		}
	}
}
