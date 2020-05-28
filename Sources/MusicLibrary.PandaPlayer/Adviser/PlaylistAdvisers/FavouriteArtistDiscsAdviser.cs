using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	internal class FavouriteArtistDiscsAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser discAdviser;
		private readonly ILogger<FavouriteArtistDiscsAdviser> logger;
		private readonly IReadOnlyCollection<string> favouriteArtists;

		private bool checkedArtists;

		public FavouriteArtistDiscsAdviser(IPlaylistAdviser discAdviser, ILogger<FavouriteArtistDiscsAdviser> logger, IOptions<FavouriteArtistsAdviserSettings> options)
		{
			this.discAdviser = discAdviser ?? throw new ArgumentNullException(nameof(discAdviser));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.favouriteArtists = options.Value?.FavouriteArtists ?? throw new ArgumentNullException(nameof(options));
		}

		public IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo)
		{
			if (!favouriteArtists.Any())
			{
				logger.LogWarning("No favourite artists are set");
				return Enumerable.Empty<AdvisedPlaylist>();
			}

			var discsList = discs.ToList();

			if (!checkedArtists)
			{
				CheckConfigurationForFavouriteArtists(discsList);
				checkedArtists = true;
			}

			var favouriteArtistDiscs = discsList
				.Where(disc => disc.GetSoloArtist() != null && favouriteArtists.Any(fa => String.Equals(fa, disc.GetSoloArtist().Name, StringComparison.Ordinal)));

			var artistOrders = favouriteArtistDiscs.GroupBy(d => d.GetSoloArtist().Id)
				.Select(g => (artistId: g.Key, playbacksPassed: g.Min(playbacksInfo.GetPlaybacksPassed)))
				.ToDictionary(x => x.artistId, x => x.playbacksPassed);

			// Selecting first advised disc for each artist. Artists are ordered by last playback.
			return discAdviser.Advise(discsList, playbacksInfo)
				.Where(ap => GetDiscFavouriteSoloArtist(ap.Disc, artistOrders) != null)
				.GroupBy(ap => ap.Disc.GetSoloArtist().Id)
				.OrderByDescending(g => artistOrders[g.Key])
				.Select(g => g.First())
				.Select(a => AdvisedPlaylist.ForFavouriteArtistDisc(a.Disc));
		}

		private static ArtistModel GetDiscFavouriteSoloArtist(DiscModel disc, IReadOnlyDictionary<ItemId, int> favouriteArtists)
		{
			var soloArtist = disc.GetSoloArtist();
			if (soloArtist == null)
			{
				return null;
			}

			return favouriteArtists.ContainsKey(soloArtist.Id) ? soloArtist : null;
		}

		private void CheckConfigurationForFavouriteArtists(IEnumerable<DiscModel> discs)
		{
			// Checking that library contains all configured favourite artists.
			var activeDiscs = discs.Where(disc => !disc.IsDeleted);
			var soloArtists = activeDiscs
				.Select(disc => disc.GetSoloArtist())
				.Where(artist => artist != null)
				.Select(artist => artist.Name)
				.ToHashSet();

			var missingArtists = favouriteArtists.Where(a => !soloArtists.Contains(a)).ToList();
			if (missingArtists.Any())
			{
				logger.LogWarning("Following favourite artist(s) are missing in the library: {MissingFavouriteArtists}", String.Join(", ", missingArtists));
			}
		}
	}
}
