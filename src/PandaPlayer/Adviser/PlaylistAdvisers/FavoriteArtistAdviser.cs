using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class FavoriteArtistAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser rankBasedAdviser;
		private readonly ILogger<FavoriteArtistAdviser> logger;
		private readonly IReadOnlyCollection<string> favoriteArtists;

		private bool CheckedArtists { get; set; }

		public FavoriteArtistAdviser(IPlaylistAdviser rankBasedAdviser, ILogger<FavoriteArtistAdviser> logger, IOptions<FavoriteArtistsAdviserSettings> options)
		{
			this.rankBasedAdviser = rankBasedAdviser ?? throw new ArgumentNullException(nameof(rankBasedAdviser));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			favoriteArtists = options.Value?.FavoriteArtists ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			if (!favoriteArtists.Any())
			{
				logger.LogWarning("No favorite artists are set");
				return new List<AdvisedPlaylist>();
			}

			var adviseGroupsList = adviseGroups.ToList();
			var adviseSets = adviseGroupsList.SelectMany(x => x.AdviseSets).ToList();

			if (!CheckedArtists)
			{
				CheckConfigurationForFavoriteArtists(adviseSets);
				CheckedArtists = true;
			}

			// We also include deleted advise sets here for correct calculation of passed playbacks.
			var favoriteArtistAdviseSets = adviseSets
				.Where(adviseSet => GetSoloArtist(adviseSet) != null && favoriteArtists.Any(fa => String.Equals(fa, GetSoloArtist(adviseSet).Name, StringComparison.Ordinal)));

			var artistOrders = favoriteArtistAdviseSets.GroupBy(adviseSet => GetSoloArtist(adviseSet).Id)
				.Select(g => (artistId: g.Key, playbacksPassed: g.Min(playbacksInfo.GetPlaybacksPassed)))
				.ToDictionary(x => x.artistId, x => x.playbacksPassed);

			// Selecting first advised set for each artist. Artists are ordered by last playback.
			return (await rankBasedAdviser.Advise(adviseGroupsList, playbacksInfo, cancellationToken))
				.Select(x => new
				{
					x.AdviseSet,
					SoloArtist = GetAdviseSetFavoriteSoloArtist(x.AdviseSet, artistOrders),
				})
				.Where(x => x.SoloArtist != null)
				.GroupBy(x => x.SoloArtist.Id)
				.OrderByDescending(g => artistOrders[g.Key])
				.Select(g => g.First())
				.Select(x => AdvisedPlaylist.ForFavoriteArtistAdviseSet(x.AdviseSet))
				.ToList();
		}

		private static ArtistModel GetAdviseSetFavoriteSoloArtist(AdviseSetContent adviseSet, IReadOnlyDictionary<ItemId, int> favoriteArtists)
		{
			var soloArtist = adviseSet.Discs
				.Select(x => x.SoloArtist)
				.Distinct(new ArtistEqualityComparer())
				.SingleOrDefault();

			if (soloArtist == null)
			{
				return null;
			}

			return favoriteArtists.ContainsKey(soloArtist.Id) ? soloArtist : null;
		}

		private void CheckConfigurationForFavoriteArtists(IEnumerable<AdviseSetContent> adviseSets)
		{
			// Checking that library contains all configured favorite artists.
			var soloArtists = adviseSets
				.Where(adviseSet => !adviseSet.IsDeleted)
				.Select(GetSoloArtist)
				.Where(artist => artist != null)
				.Select(artist => artist.Name)
				.ToHashSet();

			var missingArtists = favoriteArtists.Where(a => !soloArtists.Contains(a)).ToList();
			if (missingArtists.Any())
			{
				logger.LogWarning("Following favorite artist(s) are missing in the library: {MissingFavoriteArtists}", String.Join(", ", missingArtists));
			}
		}

		private static ArtistModel GetSoloArtist(AdviseSetContent adviseSetContent)
		{
			return adviseSetContent.Discs
				.Select(x => x.SoloArtist)
				.UniqueOrDefault(new ArtistEqualityComparer());
		}
	}
}
