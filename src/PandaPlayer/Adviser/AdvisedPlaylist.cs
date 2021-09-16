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
				.Select(x => x.AdviseSetInfo?.AdviseSet)
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
			if (adviseSet.Discs.Count > 1 && adviseSet.Discs.Any(x => x.AdviseSetInfo == null))
			{
				throw new InvalidOperationException($"Some discs from {nameof(AdviseSetContent)} do not have advise set assigned");
			}

			return adviseSet.Discs
				.OrderBy(x => x.AdviseSetInfo?.Order)
				.SelectMany(x => x.ActiveSongs)
				.ToList();
		}
	}
}
