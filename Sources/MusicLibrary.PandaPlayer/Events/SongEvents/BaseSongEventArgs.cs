using System;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
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
