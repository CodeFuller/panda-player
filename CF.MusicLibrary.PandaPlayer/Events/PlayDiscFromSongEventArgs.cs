using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	internal class PlayDiscFromSongEventArgs : EventArgs
	{
		public Song Song { get; }

		public PlayDiscFromSongEventArgs(Song song)
		{
			Song = song;
		}
	}
}
