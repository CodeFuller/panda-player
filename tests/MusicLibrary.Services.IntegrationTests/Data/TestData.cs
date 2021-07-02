namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class TestData
	{
		public TestData(string libraryStorageRoot)
		{
			FillDiscs();
			FillSongs(libraryStorageRoot);
		}
	}
}
