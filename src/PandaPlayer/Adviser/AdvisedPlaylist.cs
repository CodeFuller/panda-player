using System;
using System.Collections.Generic;
using System.Linq;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser
{
	internal class AdvisedPlaylist
	{
		public AdvisedPlaylistType AdvisedPlaylistType { get; private init; }

		public string Title { get; private init; }

		public IReadOnlyCollection<SongModel> Songs { get; private init; }

		public AdviseSetContent AdviseSet { get; private init; }

		private AdvisedPlaylist()
		{
		}

		public static AdvisedPlaylist ForAdviseSet(AdviseSetContent adviseSet)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.AdviseSet,
				Title = GetTitleForAdviseSet(adviseSet),
				Songs = GetAdviseSetSongs(adviseSet),
				AdviseSet = adviseSet,
			};
		}

		public static AdvisedPlaylist ForFavoriteArtistAdviseSet(AdviseSetContent adviseSet)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.FavoriteArtistAdviseSet,
				Title = "*** " + GetTitleForAdviseSet(adviseSet),
				Songs = GetAdviseSetSongs(adviseSet),
				AdviseSet = adviseSet,
			};
		}

		public static AdvisedPlaylist ForHighlyRatedSongs(IEnumerable<SongModel> songs)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.HighlyRatedSongs,
				Title = "Highly Rated Songs",
				Songs = songs.ToList(),
			};
		}

		private static string GetTitleForAdviseSet(AdviseSetContent adviseSet)
		{
			var commonAdviseSet = adviseSet.Discs
				.Select(x => x.AdviseSet)
				.Distinct(new AdviseSetEqualityComparer())
				.Single();

			if (commonAdviseSet != null)
			{
				return commonAdviseSet.Name;
			}

			// We safely call Single() on discs.
			// The only case when AdviseSetContent contains multiple discs is when they all have common advise set assigned.
			// This case is covered by above if branch.
			var disc = adviseSet.Discs.Single();
			return $"{disc.Folder.Name} / {disc.Title}";
		}

		private static IReadOnlyCollection<SongModel> GetAdviseSetSongs(AdviseSetContent adviseSet)
		{
			var discsSortedById = adviseSet.Discs.OrderBy(x => x.Id.Value).ToList();
			var discsSortedByTreeTitle = adviseSet.Discs.OrderBy(x => x.TreeTitle, StringComparer.InvariantCultureIgnoreCase).ToList();

			if (!discsSortedById.SequenceEqual(discsSortedByTreeTitle))
			{
				throw new InvalidOperationException($"Can not guess discs order within advise set '{adviseSet.Id}'");
			}

			return discsSortedById.SelectMany(x => x.ActiveSongs).ToList();
		}
	}
}
