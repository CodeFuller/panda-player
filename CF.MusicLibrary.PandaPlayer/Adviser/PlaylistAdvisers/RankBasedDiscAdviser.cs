﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;
using CF.MusicLibrary.Universal.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class RankBasedDiscAdviser : IPlaylistAdviser
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