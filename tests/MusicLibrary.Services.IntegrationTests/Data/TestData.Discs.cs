using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.IntegrationTests.Extensions;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public DiscModel NormalDisc { get; private set; }

		public DiscModel DiscWithNullValues { get; private set; }

		public DiscModel DeletedDisc { get; private set; }

		private void FillDiscs(string libraryStorageRoot)
		{
			NormalDisc = new()
			{
				Id = new ItemId("1"),
				Folder = ArtistFolder,
				Year = 2004,
				Title = "Planet Of The Apes - Best Of Guano Apes (CD 1)",
				TreeTitle = "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)",
				AlbumTitle = "Planet Of The Apes - Best Of Guano Apes",
				Images = new List<DiscImageModel>
				{
					new()
					{
						Id = new ItemId("1"),
						TreeTitle = "cover.jpg",
						ImageType = DiscImageType.Cover,
						Size = 15843,
						Checksum = 0x852610AB,
						ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/cover.jpg".ToContentUri(libraryStorageRoot),
					},
				},
			};

			NormalDisc.Images.Single().Disc = NormalDisc;

			DiscWithNullValues = new()
			{
				Id = new ItemId("2"),
				Folder = ArtistFolder,
				Title = "Disc With Null Values",
				TreeTitle = "Disc With Null Values (CD 1)",
			};

			DeletedDisc = new()
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
