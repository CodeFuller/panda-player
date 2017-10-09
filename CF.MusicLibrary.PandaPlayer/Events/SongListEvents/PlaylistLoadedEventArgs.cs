using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistLoadedEventArgs : BaseSongListEventArgs
	{
		public PlaylistLoadedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
		}
	}
}
