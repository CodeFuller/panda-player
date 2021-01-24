using System.Collections.ObjectModel;
using System.Linq;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.Shared.Extensions;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankedDiscGroup
	{
		private readonly string discGroupId;

		public Collection<RankedDisc> RankedDiscs { get; }

		// Deleted discs are also included
		public int PlaybacksPassed => RankedDiscs.Select(d => d.PlaybacksPassed).Min();

		public double Rating => RankedDiscs.Where(rd => !rd.Disc.IsDeleted).Select(rd => rd.Rating).Average();

		public RankedDiscGroup(DiscGroup discGroup, PlaybacksInfo playbacksInfo)
		{
			discGroupId = discGroup.Id;
			RankedDiscs = discGroup.Discs.Select(d => new RankedDisc(d, playbacksInfo.GetPlaybacksPassed(d))).ToCollection();
		}

		public DiscGroup BuildDiscGroup()
		{
			var discGroup = new DiscGroup(discGroupId);
			foreach (var rd in RankedDiscs.Select(rd => rd.Disc))
			{
				discGroup.AddDisc(rd);
			}

			return discGroup;
		}
	}
}
