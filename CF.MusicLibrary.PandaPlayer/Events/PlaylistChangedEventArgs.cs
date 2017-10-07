using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistChangedEventArgs : BaseSongListEventArgs
	{
		private readonly ISongPlaylistViewModel playlist;

		public Song CurrentSong => playlist.CurrentSong;

		public PlaylistChangedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
			this.playlist = playlist;
		}
	}
}
