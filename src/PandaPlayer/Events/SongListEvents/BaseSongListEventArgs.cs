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

		protected IEnumerable<DiscModel> Discs => Songs.Select(song => song.Disc);

		protected DiscModel UniqueDisc => Discs.UniqueOrDefault(new DiscEqualityComparer());

		protected AdviseSetModel UniqueAdviseSet => Discs
			.Select(disc => disc.AdviseSetInfo?.AdviseSet)
			.UniqueOrDefault(new AdviseSetEqualityComparer());

		protected BaseSongListEventArgs(IEnumerable<SongModel> songs)
		{
			Songs = songs?.ToList() ?? throw new ArgumentNullException(nameof(songs));
		}
	}
}
