using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public GenreModel Genre1 { get; } = new()
		{
			Id = new ItemId("1"),
			Name = "Alternative Rock",
		};

		public GenreModel Genre2 { get; } = new()
		{
			Id = new ItemId("2"),
			Name = "Rock",
		};
	}
}
