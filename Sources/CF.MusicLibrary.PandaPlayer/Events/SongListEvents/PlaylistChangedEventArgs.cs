using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
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
