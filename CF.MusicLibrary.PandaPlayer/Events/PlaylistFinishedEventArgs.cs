using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistFinishedEventArgs : BaseSongListEventArgs
	{
		public PlaylistFinishedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
		}
	}
}
