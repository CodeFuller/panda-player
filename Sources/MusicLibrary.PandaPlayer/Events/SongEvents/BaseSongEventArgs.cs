using System;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	public abstract class BaseSongEventArgs : EventArgs
	{
		public Song Song { get; }

		protected BaseSongEventArgs(Song song)
		{
			Song = song;
		}
	}
}
