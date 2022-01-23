using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId Genre1Id => new("1");

		public static ItemId Genre2Id => new("2");

		public static ItemId Genre3Id => new("3");

		public GenreModel Genre1 { get; } = new()
		{
			Id = Genre1Id,
			Name = "Punk Rock",
		};

		public GenreModel Genre2 { get; } = new()
		{
			Id = Genre2Id,
			Name = "Alternative Rock",
		};

		public GenreModel Genre3 { get; } = new()
		{
			Id = Genre3Id,
			Name = "Empty Genre",
		};
	}
}
