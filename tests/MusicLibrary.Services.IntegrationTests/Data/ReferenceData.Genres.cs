using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public static ItemId Genre1Id => new("1");

		public static ItemId Genre2Id => new("2");

		public GenreModel Genre1 { get; } = new()
		{
			Id = Genre1Id,
			Name = "Alternative Rock",
		};

		public GenreModel Genre2 { get; } = new()
		{
			Id = Genre2Id,
			Name = "Rock",
		};
	}
}
