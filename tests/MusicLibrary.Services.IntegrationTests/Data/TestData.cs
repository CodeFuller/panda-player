namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public TestData(string libraryStorageRoot)
		{
			FillDiscs(libraryStorageRoot);
			FillSongs(libraryStorageRoot);
		}
	}
}
