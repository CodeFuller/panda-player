using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events
{
	public class NewSongPlaybackStartedEventArgs : EventArgs
	{
		public Song Song { get; }

		public NewSongPlaybackStartedEventArgs(Song song)
		{
			Song = song;
		}
	}
}
