using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IAdviseRankCalculator
	{
		double CalculateSongRank(SongModel song, PlaybacksInfo playbacksInfo);

		double CalculateDiscRank(DiscModel disc, PlaybacksInfo playbacksInfo);

		double CalculateDiscGroupRank(RankedDiscGroup discGroup);
	}
}
