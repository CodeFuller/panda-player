using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Grouping;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	internal class RankedDiscGroup
	{
		private readonly DiscGroup parentDiscGroup;

		public Collection<RankedDisc> RankedDiscs { get; }

		// Deleted discs are also included
		public int PlaybacksPassed => RankedDiscs.Select(d => d.PlaybacksPassed).Min();

		public double Rating => RankedDiscs.Where(rd => !rd.Disc.IsDeleted).Select(rd => rd.Rating).Average();

		public RankedDiscGroup(DiscGroup discGroup)
		{
			parentDiscGroup = discGroup;
			RankedDiscs = discGroup.Discs.Select(d => new RankedDisc(d)).ToCollection();
		}

		public DiscGroup BuildDiscGroup()
		{
			DiscGroup discGroup = new DiscGroup(parentDiscGroup.Id);
			foreach (var rd in RankedDiscs.Select(rd => rd.Disc))
			{
				discGroup.Discs.Add(rd);
			}

			return discGroup;
		}
	}
}
