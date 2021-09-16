using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class RankBasedAdviser : IPlaylistAdviser
	{
		private readonly IAdviseContentSorter adviseContentSorter;

		public RankBasedAdviser(IAdviseContentSorter adviseContentSorter)
		{
			this.adviseContentSorter = adviseContentSorter ?? throw new ArgumentNullException(nameof(adviseContentSorter));
		}

		public Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			var playlists = GetSortedAdviseSets(adviseGroups, playbacksInfo)
				.Select(AdvisedPlaylist.ForAdviseSet)
				.ToList();

			return Task.FromResult<IReadOnlyCollection<AdvisedPlaylist>>(playlists);
		}

		private IEnumerable<AdviseSetContent> GetSortedAdviseSets(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo)
		{
			var activeAdviseGroups = adviseGroups.Where(x => !x.IsDeleted);

			foreach (var adviseGroup in adviseContentSorter.SortAdviseGroups(activeAdviseGroups, playbacksInfo))
			{
				var activeAdviseSets = adviseGroup.AdviseSets.Where(x => !x.IsDeleted);
				yield return adviseContentSorter.SortAdviseSets(activeAdviseSets, playbacksInfo).First();
			}
		}
	}
}
