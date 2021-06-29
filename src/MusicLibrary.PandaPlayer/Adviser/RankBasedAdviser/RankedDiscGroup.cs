using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankedDiscGroup
	{
		private readonly Lazy<int> playbacksPassedLazy;

		public DiscGroup DiscGroup { get; }

		public IReadOnlyCollection<DiscModel> Discs => DiscGroup.Discs;

		// Deleted discs are also included
		public int PlaybacksPassed => playbacksPassedLazy.Value;

		public double Rating => Discs.Where(d => !d.IsDeleted).Select(d => d.GetRating()).Average();

		public RankedDiscGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo)
		{
			DiscGroup = discGroup ?? throw new ArgumentNullException(nameof(discGroup));

			// We use Lazy because Discs could be empty. In this case Lazy value will not be requested.
			// Otherwise, Min() will throw on empty sequence.
			playbacksPassedLazy = new Lazy<int>(() => Discs.Select(playbacksInfo.GetPlaybacksPassed).Min());
		}
	}
}
