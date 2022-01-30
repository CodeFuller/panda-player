namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public ReferenceData(string libraryStorageRoot, bool fillSongPlaybacks)
		{
			FillFolders();
			FillDiscs(libraryStorageRoot);
			FillSongs(libraryStorageRoot, fillSongPlaybacks);
		}
	}
}
