using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Universal.Interfaces;

namespace CF.MusicLibrary.DiscAdviser
{
	public class RankBasedDiscAdviser : IDiscAdviser
	{
		private readonly IDiscGroupper discGroupper;
		private readonly IDiscGroupSorter discGroupSorter;

		public RankBasedDiscAdviser(IDiscGroupper discGroupper, IDiscGroupSorter discGroupSorter)
		{
			if (discGroupper == null)
			{
				throw new ArgumentNullException(nameof(discGroupper));
			}
			if (discGroupSorter == null)
			{
				throw new ArgumentNullException(nameof(discGroupSorter));
			}

			this.discGroupper = discGroupper;
			this.discGroupSorter = discGroupSorter;
		}

		public Collection<Disc> AdviseDiscs(DiscLibrary library)
		{
			var discGroups = discGroupper.GroupLibraryDiscs(library)
				.Where(dg => !dg.Discs.All(d => d.IsDeleted));

			Collection<Disc> advisedDiscs = new Collection<Disc>();
			foreach (var group in discGroupSorter.SortDiscGroups(discGroups))
			{
				var disc = discGroupSorter.SortDiscsWithinGroup(group).FirstOrDefault();
				if (disc != null)
				{
					advisedDiscs.Add(disc);
				}
			}

			return advisedDiscs;
		}
	}
}
