using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CF.MusicLibrary.Local
{
	public static class MyLocalLibraryNames
	{
		public static string Soundtracks => "Soundtracks";

		public static string Collections => "Сборники";

		public static Collection<string> ArtistCategories { get; } = new Collection<string>
		{
			"Belarussian",
			"Foreign",
			"Russian",
		};

		private static Collection<string> RussianRockDirectories { get; } = new Collection<string>
		{
			"Maxidrom",
			"Нашествие",
			"Чартова дюжина",
		};

		public static bool IsArtistCategory(string category)
		{
			return ArtistCategories.Any(d => String.Equals(d, category, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsRussianRockCollectionDirectory(string directoryName)
		{
			return RussianRockDirectories.Any(d => String.Equals(d, directoryName, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsBestCollectionDirectory(string directoryName)
		{
			return String.Equals(directoryName, "Best", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsEurovisionDirectory(string directoryName)
		{
			return String.Equals(directoryName, "Евровидение", StringComparison.OrdinalIgnoreCase);
		}
	}
}
