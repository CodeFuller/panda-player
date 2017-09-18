using System;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlaylistFinishedEventArgs : EventArgs
	{
		public ISongPlaylistViewModel Playlist { get; }

		public PlaylistFinishedEventArgs(ISongPlaylistViewModel playlist)
		{
			Playlist = playlist;
		}
	}
}
