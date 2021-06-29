using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;

namespace MusicLibrary.PandaPlayer.Adviser.Internal
{
	internal class AdviseRankCalculator : IAdviseRankCalculator
	{
		private const double MaxRank = Double.MaxValue;

		private readonly IAdviseFactorsProvider adviseFactorsProvider;

		public AdviseRankCalculator(IAdviseFactorsProvider adviseFactorsProvider)
		{
			this.adviseFactorsProvider = adviseFactorsProvider ?? throw new ArgumentNullException(nameof(adviseFactorsProvider));
		}

		public double CalculateSongRank(SongModel song, PlaybacksInfo playbacksInfo)
		{
			if (!song.LastPlaybackTime.HasValue)
			{
				return MaxRank;
			}

			var factorForRating = adviseFactorsProvider.GetFactorForRating(song.GetRatingOrDefault());
			var factorForPlaybacksAge = adviseFactorsProvider.GetFactorForPlaybackAge(playbacksInfo.GetPlaybacksPassed(song));

			return factorForRating * factorForPlaybacksAge;
		}

		public double CalculateDiscRank(DiscModel disc, PlaybacksInfo playbacksInfo)
		{
			if (!disc.GetLastPlaybackTime().HasValue)
			{
				return MaxRank;
			}

			var playbacksPassed = playbacksInfo.GetPlaybacksPassed(disc);
			return adviseFactorsProvider.GetFactorForAverageRating(disc.GetRating()) * adviseFactorsProvider.GetFactorForPlaybackAge(playbacksPassed);
		}

		public double CalculateDiscGroupRank(RankedDiscGroup discGroup)
		{
			return discGroup.PlaybacksPassed == Int32.MaxValue ? MaxRank :
				adviseFactorsProvider.GetFactorForGroupDiscsNumber(discGroup.Discs.Count(d => !d.IsDeleted)) *
				adviseFactorsProvider.GetFactorForAverageRating(discGroup.Rating) *
				adviseFactorsProvider.GetFactorForPlaybackAge(discGroup.PlaybacksPassed);
		}
	}
}
