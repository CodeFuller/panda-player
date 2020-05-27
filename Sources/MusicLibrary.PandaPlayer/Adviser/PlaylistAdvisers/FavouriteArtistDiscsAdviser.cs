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
				// Checking that library contains all configured favourite artists.
				var discArtists = new HashSet<string>(discsList.Where(d => d.GetSoloArtist() != null).Select(d => d.GetSoloArtist().Name).Distinct());
				var missingArtists = favouriteArtists.Where(a => !discArtists.Contains(a)).ToList();
				if (missingArtists.Any())
				{
					logger.LogWarning("Following favourite artist(s) are missing in the library: {MissingFavouriteArtists}", String.Join(", ", missingArtists));
				}

				checkedArtists = true;
			}

			var favouriteArtistDiscs = discsList
				.Where(d => d.GetSoloArtist() != null && favouriteArtists.Any(fa => String.Equals(fa, d.GetSoloArtist().Name, StringComparison.Ordinal)));

			var artistOrders = favouriteArtistDiscs.GroupBy(d => d.GetSoloArtist().Id)
				.Select(g => (artistId: g.Key, playbacksPassed: g.Min(playbacksInfo.GetPlaybacksPassed)))
				.ToDictionary(x => x.artistId, x => x.playbacksPassed);

			// Selecting first advised disc for each artist. Artists are ordered by last playback.
			return discAdviser.Advise(discsList, playbacksInfo)
				.Where(ap => ap.Disc.GetSoloArtist() != null && artistOrders.ContainsKey(ap.Disc.GetSoloArtist().Id))
				.GroupBy(ap => ap.Disc.GetSoloArtist().Id)
				.OrderByDescending(g => artistOrders[g.Key])
				.Select(g => g.First())
				.Select(a => AdvisedPlaylist.ForFavouriteArtistDisc(a.Disc));
		}
	}
}
