using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IAdviseRankCalculator
	{
		double CalculateSongRank(SongModel song, PlaybacksInfo playbacksInfo);

		double CalculateAdviseSetRank(AdviseSetContent adviseSet, PlaybacksInfo playbacksInfo);

		double CalculateAdviseGroupRank(RankedAdviseGroup adviseGroup);
	}
}
