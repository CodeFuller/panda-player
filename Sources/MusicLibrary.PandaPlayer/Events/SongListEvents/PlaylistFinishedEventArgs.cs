using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistFinishedEventArgs : BaseSongListEventArgs
	{
		public PlaylistFinishedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
		}
	}
}
