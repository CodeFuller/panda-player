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
	internal class FavoriteAdviseGroupsAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser rankBasedAdviser;

		public FavoriteAdviseGroupsAdviser(IPlaylistAdviser rankBasedAdviser)
		{
			this.rankBasedAdviser = rankBasedAdviser ?? throw new ArgumentNullException(nameof(rankBasedAdviser));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			var adviseGroupsList = adviseGroups.ToList();

			var adviseSetToAdviseGroupMap = adviseGroupsList
				.SelectMany(adviseGroup => adviseGroup.AdviseSets.Select(adviseSet => (adviseGroup, adviseSet)))
				.ToDictionary(x => x.adviseSet.Id, x => x.adviseGroup);

			var advisedPlaylists = await rankBasedAdviser.Advise(adviseGroupsList, playbacksInfo, cancellationToken);
			return advisedPlaylists.Select(x => x.AdviseSet)
			.GroupBy(x => adviseSetToAdviseGroupMap[x.Id], new AdviseGroupContentEqualityComparer())
			.Where(g => g.Key.IsFavorite)
			.OrderByDescending(g => playbacksInfo.GetPlaybacksPassed(g.Key))
			.Select(g => g.First())
			.Select(AdvisedPlaylist.ForAdviseSetFromFavoriteAdviseGroup)
			.ToList();
		}
	}
}
