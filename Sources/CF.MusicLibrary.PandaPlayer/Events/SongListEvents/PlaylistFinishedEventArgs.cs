using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistFinishedEventArgs : BaseSongListEventArgs
	{
		public PlaylistFinishedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
		}
	}
}
