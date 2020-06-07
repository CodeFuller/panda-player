namespace MusicLibrary.Services.Interfaces
{
	public interface IDiscTitleToAlbumMapper
	{
		string GetAlbumTitleFromDiscTitle(string discTitle);
	}
}
