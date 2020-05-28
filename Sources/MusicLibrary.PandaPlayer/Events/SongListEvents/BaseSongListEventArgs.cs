using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Extensions;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public abstract class BaseSongListEventArgs : EventArgs
	{
		public IReadOnlyCollection<SongModel> Songs { get; }

		public DiscModel Disc => Songs.UniqueOrDefault(new SongEqualityComparer())?.Disc;

		protected BaseSongListEventArgs(IEnumerable<SongModel> songs)
		{
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
