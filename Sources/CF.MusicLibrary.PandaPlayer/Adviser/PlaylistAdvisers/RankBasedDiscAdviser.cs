using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser.Grouping;
using CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class RankBasedDiscAdviser : IPlaylistAdviser
	{
		private readonly IDiscGroupper discGroupper;
		private readonly IDiscGroupSorter discGroupSorter;

		public RankBasedDiscAdviser(IDiscGroupper discGroupper, IDiscGroupSorter discGroupSorter)
		{
			this.discGroupper = discGroupper ?? throw new ArgumentNullException(nameof(discGroupper));
			this.discGroupSorter = discGroupSorter ?? throw new ArgumentNullException(nameof(discGroupSorter));
		}

		public IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			var discGroups = discGroupper.GroupLibraryDiscs(discLibrary)
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

			return advisedDiscs.Select(AdvisedPlaylist.ForDisc);
		}
	}
}
