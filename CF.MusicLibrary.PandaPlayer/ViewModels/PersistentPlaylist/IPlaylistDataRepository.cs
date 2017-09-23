namespace CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	public interface IPlaylistDataRepository
	{
		void Save(PlaylistData playlistData);

		PlaylistData Load();

		void Purge();
	}
}
