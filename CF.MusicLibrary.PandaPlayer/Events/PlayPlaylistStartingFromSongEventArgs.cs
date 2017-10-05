using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class PlayPlaylistStartingFromSongEventArgs : EventArgs
	{
		public Song Song { get; }

		public PlayPlaylistStartingFromSongEventArgs(Song song)
		{
			Song = song;
		}
	}
}
