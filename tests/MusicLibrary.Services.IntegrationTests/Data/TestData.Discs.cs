using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public DiscModel Disc1 { get; private set; }

		public DiscModel Disc2 { get; private set; }

		public DiscModel Disc3 { get; private set; }

		private void FillDiscs()
		{
			Disc1 = new()
			{
				Id = new ItemId("1"),
				Folder = ArtistFolder,
				Year = 1997,
				Title = "Planet Of The Apes - Best Of Guano Apes (CD 1)",
				TreeTitle = "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)",
				AlbumTitle = "Planet Of The Apes - Best Of Guano Apes",
			};

			Disc2 = new()
			{
				Id = new ItemId("2"),
				Folder = ArtistFolder,
				Title = "Disc With Null Values",
				TreeTitle = "Disc With Null Values (CD 1)",
			};

			Disc3 = new()
			{
				Id = new ItemId("3"),
				Folder = ArtistFolder,
				Year = 2021,
				Title = "Deleted Disc",
				TreeTitle = "2021 - Deleted Disc",
				AlbumTitle = "Deleted Disc",
			};
		}
	}
}
