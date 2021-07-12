using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.RankBasedAdviser;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IAdviseRankCalculator
	{
		double CalculateSongRank(SongModel song, PlaybacksInfo playbacksInfo);

		double CalculateDiscRank(DiscModel disc, PlaybacksInfo playbacksInfo);

		double CalculateDiscGroupRank(RankedDiscGroup discGroup);
	}
}
