using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Events.SongListEvents
{
	public abstract class BaseSongListEventArgs : EventArgs
	{
		public IReadOnlyCollection<SongModel> Songs { get; }

		public DiscModel Disc => Songs
			.Select(song => song.Disc)
			.UniqueOrDefault(new DiscEqualityComparer());

		protected BaseSongListEventArgs(IEnumerable<SongModel> songs)
		{
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
