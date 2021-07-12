using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId Artist1Id => new("1");

		public static ItemId Artist2Id => new("2");

		public static ItemId NextArtistId => new("3");

		public ArtistModel Artist1 { get; } = new()
		{
			Id = Artist1Id,
			Name = "Guano Apes",
		};

		public ArtistModel Artist2 { get; } = new()
		{
			Id = Artist2Id,
			Name = "Neuro Dubel",
		};
	}
}
