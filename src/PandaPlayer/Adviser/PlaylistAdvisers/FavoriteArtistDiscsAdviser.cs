using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.Settings;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class FavoriteArtistDiscsAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser discAdviser;
		private readonly ILogger<FavoriteArtistDiscsAdviser> logger;
		private readonly IReadOnlyCollection<string> favoriteArtists;

		private bool CheckedArtists { get; set; }

		public FavoriteArtistDiscsAdviser(IPlaylistAdviser discAdviser, ILogger<FavoriteArtistDiscsAdviser> logger, IOptions<FavoriteArtistsAdviserSettings> options)
		{
			this.discAdviser = discAdviser ?? throw new ArgumentNullException(nameof(discAdviser));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.favoriteArtists = options.Value?.FavoriteArtists ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken)
		{
			if (!favoriteArtists.Any())
			{
				logger.LogWarning("No favorite artists are set");
				return new List<AdvisedPlaylist>();
			}

			var discsList = discs.ToList();

			if (!CheckedArtists)
			{
				CheckConfigurationForFavoriteArtists(discsList);
				CheckedArtists = true;
			}

			var favoriteArtistDiscs = discsList
				.Where(disc => disc.SoloArtist != null && favoriteArtists.Any(fa => String.Equals(fa, disc.SoloArtist.Name, StringComparison.Ordinal)));

			var artistOrders = favoriteArtistDiscs.GroupBy(d => d.SoloArtist.Id)
				.Select(g => (artistId: g.Key, playbacksPassed: g.Min(playbacksInfo.GetPlaybacksPassed)))
				.ToDictionary(x => x.artistId, x => x.playbacksPassed);

			// Selecting first advised disc for each artist. Artists are ordered by last playback.
			return (await discAdviser.Advise(discsList, playbacksInfo, cancellationToken))
				.Where(ap => GetDiscFavoriteSoloArtist(ap.Disc, artistOrders) != null)
				.GroupBy(ap => ap.Disc.SoloArtist.Id)
				.OrderByDescending(g => artistOrders[g.Key])
				.Select(g => g.First())
				.Select(a => AdvisedPlaylist.ForFavoriteArtistDisc(a.Disc))
				.ToList();
		}

		private static ArtistModel GetDiscFavoriteSoloArtist(DiscModel disc, IReadOnlyDictionary<ItemId, int> favoriteArtists)
		{
			var soloArtist = disc.SoloArtist;
			if (soloArtist == null)
			{
				return null;
			}

			return favoriteArtists.ContainsKey(soloArtist.Id) ? soloArtist : null;
		}

		private void CheckConfigurationForFavoriteArtists(IEnumerable<DiscModel> discs)
		{
			// Checking that library contains all configured favorite artists.
			var activeDiscs = discs.Where(disc => !disc.IsDeleted);
			var soloArtists = activeDiscs
				.Select(disc => disc.SoloArtist)
				.Where(artist => artist != null)
				.Select(artist => artist.Name)
				.ToHashSet();

			var missingArtists = favoriteArtists.Where(a => !soloArtists.Contains(a)).ToList();
			if (missingArtists.Any())
			{
				logger.LogWarning("Following favorite artist(s) are missing in the library: {MissingFavoriteArtists}", String.Join(", ", missingArtists));
			}
		}
	}
}
