using System;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongEvents
{
	public abstract class BaseSongEventArgs : EventArgs
	{
		public SongModel Song { get; }

		protected BaseSongEventArgs(SongModel song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));
		}
	}
}
