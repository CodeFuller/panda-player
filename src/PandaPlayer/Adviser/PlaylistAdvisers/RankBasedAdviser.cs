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
		private readonly IAdviseGroupSorter adviseGroupSorter;

		public RankBasedAdviser(IAdviseGroupSorter adviseGroupSorter)
		{
			this.adviseGroupSorter = adviseGroupSorter ?? throw new ArgumentNullException(nameof(adviseGroupSorter));
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

			foreach (var adviseGroup in adviseGroupSorter.SortAdviseGroups(activeAdviseGroups, playbacksInfo))
			{
				var activeAdviseSets = adviseGroup.AdviseSets.Where(x => !x.IsDeleted);
				yield return adviseGroupSorter.SortAdviseSets(activeAdviseSets, playbacksInfo).First();
			}
		}
	}
}
