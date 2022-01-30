using PandaPlayer.Core.Models;
using PandaPlayer.Services.IntegrationTests.Extensions;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId NormalDiscId => new("1");

		public static ItemId DiscWithMissingFieldsId => new("2");

		public static ItemId DeletedDiscId => new("3");

		public static ItemId NextDiscId => new("4");

		public static ItemId DiscCoverImageId => new("1");

		public static ItemId NextDiscCoverImageId => new("2");

		public DiscModel NormalDisc { get; private set; }

		public DiscModel DiscWithMissingFields { get; private set; }

		public DiscModel DeletedDisc { get; private set; }

		private void FillDiscs(string libraryStorageRoot)
		{
			NormalDisc = new()
			{
				Id = NormalDiscId,
				AdviseGroup = DiscAdviseGroup,
				AdviseSetInfo = new AdviseSetInfo(AdviseSet1, 1),
				Year = 2010,
				Title = "Афтары правды (CD 1)",
				TreeTitle = "2010 - Афтары правды (CD 1)",
				AlbumTitle = "Афтары правды",
			};

			NormalDisc.AddImage(
				new()
				{
					Id = new ItemId("1"),
					TreeTitle = "cover.jpg",
					ImageType = DiscImageType.Cover,
					Size = 359119,
					Checksum = 2792704281,
					ContentUri =
						"Belarusian/Neuro Dubel/2010 - Афтары правды (CD 1)/cover.jpg".ToContentUri(libraryStorageRoot),
				});

			DiscWithMissingFields = new()
			{
				Id = DiscWithMissingFieldsId,
				Title = "Disc With Missing Fields",
				TreeTitle = "Disc With Missing Fields (CD 1)",
			};

			DeletedDisc = new()
			{
				Id = DeletedDiscId,
				Year = 2021,
				Title = "Deleted Disc",
				TreeTitle = "2021 - Deleted Disc",
				AlbumTitle = "Deleted Disc",
			};

			ArtistFolder.AddDisc(NormalDisc);
			ArtistFolder.AddDisc(DiscWithMissingFields);
			ArtistFolder.AddDisc(DeletedDisc);
		}
	}
}
