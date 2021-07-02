using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public ArtistModel Artist1 { get; } = new()
		{
			Id = new ItemId("1"),
			Name = "Guano Apes",
		};

		public ArtistModel Artist2 { get; } = new()
		{
			Id = new ItemId("2"),
			Name = "Neuro Dubel",
		};
	}
}
