namespace MusicLibrary.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public ReferenceData(string libraryStorageRoot)
		{
			FillDiscs(libraryStorageRoot);
			FillSongs(libraryStorageRoot);
		}
	}
}
