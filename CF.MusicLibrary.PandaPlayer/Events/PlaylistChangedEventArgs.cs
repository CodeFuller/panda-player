using System;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistChangedEventArgs : EventArgs
	{
		public ISongPlaylistViewModel Playlist { get; set; }

		public PlaylistChangedEventArgs(ISongPlaylistViewModel playlist)
		{
			Playlist = playlist;
		}
	}
}
