namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	internal static class MyLocalLibraryNames
	{
		public static string Soundtracks => "Soundtracks";

		public static string Collections => "Сборники";

		public static bool IsArtistCategory(string category)
		{
			return category == "Belarussian" || category == "Foreign" || category == "Russian";
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
