using System.Collections.ObjectModel;

namespace CF.MusicLibrary.BL.MyLocalLibrary
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

		public static Collection<string> CollectionCategories { get; } = new Collection<string>
		{
			Soundtracks,
			Collections,
		};

		public static bool IsArtistCategory(string category)
		{
			return ArtistCategories.Contains(category);
		}

		public static bool IsRussianRockCollectionDirectory(string directoryName)
		{
			return directoryName == "Maxidrom" || directoryName == "Нашествие";
		}

		public static bool IsBestCollectionDirectory(string directoryName)
		{
			return directoryName == "Best";
		}

		public static bool IsEurovisionDirectory(string directoryName)
		{
			return directoryName == "Евровидение";
		}
	}
}
