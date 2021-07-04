using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.IntegrationTests.Extensions;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId NormalDiscId => new("1");

		public static ItemId DiscWithNullValuesId => new("2");

		public static ItemId DeletedDiscId => new("3");

		public static ItemId NextDiscId => new("4");

		public static ItemId DiscCoverImageId => new("1");

		public static ItemId NextDiscCoverImageId => new("2");

		public DiscModel NormalDisc { get; private set; }

		public DiscModel DiscWithNullValues { get; private set; }

		public DiscModel DeletedDisc { get; private set; }

		private void FillDiscs(string libraryStorageRoot)
		{
			NormalDisc = new()
			{
				Id = NormalDiscId,
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
						Checksum = 2233864363,
						ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/cover.jpg".ToContentUri(libraryStorageRoot),
					},
				},
			};

			NormalDisc.Images.Single().Disc = NormalDisc;

			DiscWithNullValues = new()
			{
				Id = DiscWithNullValuesId,
				Folder = ArtistFolder,
				Title = "Disc With Null Values",
				TreeTitle = "Disc With Null Values (CD 1)",
			};

			DeletedDisc = new()
			{
				Id = DeletedDiscId,
				Folder = ArtistFolder,
				Year = 2021,
				Title = "Deleted Disc",
				TreeTitle = "2021 - Deleted Disc",
				AlbumTitle = "Deleted Disc",
			};
		}
	}
}
