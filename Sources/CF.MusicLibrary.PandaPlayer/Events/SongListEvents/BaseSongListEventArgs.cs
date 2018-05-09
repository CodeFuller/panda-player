using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class BaseSongListEventArgs : EventArgs
	{
		public IReadOnlyCollection<Song> Songs { get; }

		public Disc Disc => Songs.Select(s => s.Disc).UniqueOrDefault();

		protected BaseSongListEventArgs(IEnumerable<Song> songs)
		{
			Songs = songs.ToList();
		}
	}
}
