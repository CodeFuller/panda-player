using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Universal;

namespace CF.MusicLibrary.DiscAdviser
{
	internal class RankedDiscGroup
	{
		private readonly DiscGroup parentDiscGroup;

		public Collection<RankedDisc> RankedDiscs { get; }

		//	Deleted discs are also included
		public int PlaybacksPassed => RankedDiscs.Select(d => d.PlaybacksPassed).Min();

		public double Rating => RankedDiscs.Where(rd => !rd.Disc.IsDeleted).Select(rd => rd.Rating).Average();

		public RankedDiscGroup(DiscGroup discGroup)
		{
			parentDiscGroup = discGroup;
			RankedDiscs = discGroup.Discs.Select(d => new RankedDisc(d)).ToCollection();
		}

		public DiscGroup BuildDiscGroup()
		{
			DiscGroup discGroup = new DiscGroup(parentDiscGroup.Id, parentDiscGroup.Title);
			foreach (var rd in RankedDiscs.Select(rd => rd.Disc))
			{
				discGroup.Discs.Add(rd);
			}

			return discGroup;
		}
	}
}
