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

		public double Rating => Disc.ActiveSongs
			.Select(song => song.GetRatingOrDefault())
			.Select(rating => rating.GetRatingValueForDiscAdviser())
			.Average();

		public RankedDisc(DiscModel disc, int playbacksPassed)
		{
			Disc = disc ?? throw new ArgumentNullException(nameof(disc));
			PlaybacksPassed = playbacksPassed;
		}
	}
}
