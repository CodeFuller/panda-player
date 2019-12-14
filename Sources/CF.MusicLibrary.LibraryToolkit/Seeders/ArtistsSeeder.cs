using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;
using MusicLibraryApi.Client.Contracts.Artists;
using MusicLibraryApi.Client.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit.Seeders
{
	public class ArtistsSeeder : IArtistsSeeder
	{
		private readonly IArtistsMutation artistsMutation;

		private readonly ILogger<ArtistsSeeder> logger;

		public ArtistsSeeder(IArtistsMutation artistsMutation, ILogger<ArtistsSeeder> logger)
		{
			this.artistsMutation = artistsMutation ?? throw new ArgumentNullException(nameof(artistsMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IReadOnlyDictionary<int, int>> SeedArtists(DiscLibrary discLibrary, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding artists ...");

			var artists = new Dictionary<int, int>();
			foreach (var artist in discLibrary.AllArtists.OrderBy(a => a.Name))
			{
				var artistData = new InputArtistData(artist.Name);
				var newArtistId = await artistsMutation.CreateArtist(artistData, cancellationToken);

				artists.Add(artist.Id, newArtistId);
			}

			logger.LogInformation("Seeded {ArtistsNumber} artists", artists.Count);

			return artists;
		}
	}
}
