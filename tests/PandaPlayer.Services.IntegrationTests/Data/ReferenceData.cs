namespace PandaPlayer.Services.IntegrationTests.Data
{
	public partial class ReferenceData
	{
		public ReferenceData(string libraryStorageRoot, bool fillSongPlaybacks)
		{
			FillDiscs(libraryStorageRoot);
			FillSongs(libraryStorageRoot);

			if (fillSongPlaybacks)
			{
				FillPlaybacks();
			}
		}
	}
}
