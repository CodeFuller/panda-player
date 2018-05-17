namespace CF.MusicLibrary.Core.Interfaces
{
	public interface IDiscTitleToAlbumMapper
	{
		string GetAlbumTitleFromDiscTitle(string discTitle);

		bool AlbumTitleIsSuspicious(string albumTitle);
	}
}
