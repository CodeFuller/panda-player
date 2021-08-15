using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class RankBasedDiscAdviser : IPlaylistAdviser
	{
		private readonly IDiscClassifier discClassifier;
		private readonly IDiscGroupSorter discGroupSorter;

		public RankBasedDiscAdviser(IDiscClassifier discClassifier, IDiscGroupSorter discGroupSorter)
		{
			this.discClassifier = discClassifier ?? throw new ArgumentNullException(nameof(discClassifier));
			this.discGroupSorter = discGroupSorter ?? throw new ArgumentNullException(nameof(discGroupSorter));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			var discGroups = (await discClassifier.GroupLibraryDiscs(discs, cancellationToken))
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

			return advisedDiscs.Select(AdvisedPlaylist.ForDisc).ToList();
		}
	}
}
