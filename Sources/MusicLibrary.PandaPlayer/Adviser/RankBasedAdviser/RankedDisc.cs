using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankedDisc
	{
		public DiscModel Disc { get; }

		public int PlaybacksPassed { get; }

		public double Rating => Disc.Songs
			.Select(s => s.GetRatingOrDefault())
			.Select(r => r.GetRatingValueForDiscAdviser())
			.Average();

		public RankedDisc(DiscModel disc, int playbacksPassed)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
			PlaybacksPassed = playbacksPassed;
		}
	}
}
