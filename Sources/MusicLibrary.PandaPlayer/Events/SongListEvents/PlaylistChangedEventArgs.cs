using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistChangedEventArgs : BaseSongListEventArgs
	{
		public SongModel CurrentSong { get; }

		public PlaylistChangedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
			CurrentSong = playlist.CurrentSong;
		}
	}
}
