using System;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistLoadedEventArgs : EventArgs
	{
		public ISongPlaylistViewModel Playlist { get; }

		public PlaylistLoadedEventArgs(ISongPlaylistViewModel playlist)
		{
			Playlist = playlist;
		}
	}
}
