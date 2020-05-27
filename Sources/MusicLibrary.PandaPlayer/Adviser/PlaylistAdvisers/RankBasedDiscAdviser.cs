using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class RankBasedDiscAdviser : IPlaylistAdviser
	{
		private readonly IDiscGroupper discGroupper;
		private readonly IDiscGroupSorter discGroupSorter;

		public RankBasedDiscAdviser(IDiscGroupper discGroupper, IDiscGroupSorter discGroupSorter)
		{
			this.discGroupper = discGroupper ?? throw new ArgumentNullException(nameof(discGroupper));
			this.discGroupSorter = discGroupSorter ?? throw new ArgumentNullException(nameof(discGroupSorter));
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo)
		{
			var discGroups = discGroupper.GroupLibraryDiscs(discs)
				.Where(dg => !dg.Discs.All(d => d.IsDeleted));

			var advisedDiscs = new Collection<DiscModel>();
			foreach (var group in discGroupSorter.SortDiscGroups(discGroups, playbacksInfo))
			{
				var disc = discGroupSorter.SortDiscsWithinGroup(group, playbacksInfo).FirstOrDefault();
				if (disc != null)
				{
					advisedDiscs.Add(disc);
				}
			}

			return advisedDiscs.Select(AdvisedPlaylist.ForDisc);
		}
	}
}
