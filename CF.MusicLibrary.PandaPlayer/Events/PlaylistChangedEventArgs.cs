using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistChangedEventArgs : BaseSongListEventArgs
	{
		public Song CurrentSong { get; }

		public PlaylistChangedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
			CurrentSong = playlist.CurrentSong;
		}
	}
}
