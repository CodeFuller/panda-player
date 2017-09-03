using System.Text.RegularExpressions;

namespace CF.MusicLibrary.LibraryChecker
{
	public static class AlbumTitleChecker
	{
		private static readonly Regex CDRegex = new Regex(@"^(.+) \(CD \d+\)$", RegexOptions.Compiled);
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

		public static bool AlbumTitleIsSuspicious(string albumTitle)
		{
			if (albumTitle == null)
			{
				return false;
			}

			if (albumTitle == "Rarities")
			{
				return true;
			}

			var correctAlbumTitle = albumTitle;
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, CDRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, SingleRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, DemoRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, LiveRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, BonusRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, CompilationRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, BSidesRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, RemixesRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, RussianCompilationRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, TributeRegex);
			correctAlbumTitle = ProcessTitleRegex(correctAlbumTitle, SoundtrackRegex);

			return albumTitle != correctAlbumTitle;
		}

		private static string ProcessTitleRegex(string title, Regex regex)
		{
			var match = regex.Match(title);
			return match.Success ? match.Groups[1].Value : title;
		}
	}
}
