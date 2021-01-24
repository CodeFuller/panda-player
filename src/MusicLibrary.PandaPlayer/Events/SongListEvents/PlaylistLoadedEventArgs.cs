using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistLoadedEventArgs : BaseSongListEventArgs
	{
		public PlaylistLoadedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
		}
	}
}
