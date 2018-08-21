using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class FavouriteArtistDiscsAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser discAdviser;
		private readonly ILogger<FavouriteArtistDiscsAdviser> logger;
		private readonly ICollection<string> favouriteArtists;

		private bool checkedArtists;

		public FavouriteArtistDiscsAdviser(IPlaylistAdviser discAdviser, ILogger<FavouriteArtistDiscsAdviser> logger, IOptions<FavouriteArtistsAdviserSettings> options)
		{
			this.discAdviser = discAdviser ?? throw new ArgumentNullException(nameof(discAdviser));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.favouriteArtists = options.Value?.FavouriteArtists ?? throw new ArgumentNullException(nameof(options));
		}

		public IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			if (!checkedArtists)
			{
				// Checking that library contains all configured favourite artists.
				var discArtists = new HashSet<string>(discLibrary.Discs.Where(d => d.Artist != null).Select(d => d.Artist.Name).Distinct());
				var missingArtists = favouriteArtists.Where(a => !discArtists.Contains(a)).ToList();
				if (missingArtists.Any())
				{
					logger.LogWarning("Following favourite artist(s) are missing in the library: {MissingFavouriteArtists}", String.Join(", ", missingArtists));
				}

				checkedArtists = true;
			}

			var favouriteArtistDiscs = discLibrary.Discs
				.Where(d => d.Artist != null && favouriteArtists.Any(fa => String.Equals(fa, d.Artist.Name, StringComparison.Ordinal)));
			var artistOrders = favouriteArtistDiscs.GroupBy(d => d.Artist)
				.Select(g => new
				{
					Artist = g.Key,
					Passed = g.Min(k => k.PlaybacksPassed)
				})
				.ToDictionary(k => k.Artist, k => k.Passed);

			// Selecting first advised disc for each artist. Artists are ordered by last playback.
			return discAdviser.Advise(discLibrary)
				.Where(ap => ap.Disc.Artist != null && artistOrders.ContainsKey(ap.Disc.Artist))
				.GroupBy(ap => ap.Disc.Artist)
				.OrderByDescending(g => artistOrders[g.Key])
				.Select(g => g.First())
				.Select(a => AdvisedPlaylist.ForFavouriteArtistDisc(a.Disc));
		}
	}
}
