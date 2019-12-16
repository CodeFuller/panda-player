using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;
using MusicLibraryApi.Client.Contracts.Genres;
using MusicLibraryApi.Client.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit.Seeders
{
	public class GenresSeeder : IGenresSeeder
	{
		private readonly IGenresMutation genresMutation;

		private readonly ILogger<ArtistsSeeder> logger;

		public GenresSeeder(IGenresMutation genresMutation, ILogger<ArtistsSeeder> logger)
		{
			this.genresMutation = genresMutation ?? throw new ArgumentNullException(nameof(genresMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IReadOnlyDictionary<int, int>> SeedGenres(DiscLibrary discLibrary, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding genres ...");

			var genres = new Dictionary<int, int>();
			foreach (var genre in discLibrary.Genres.OrderBy(g => g.Name))
			{
				var genreData = new InputGenreData { Name = genre.Name };
				var newGenreId = await genresMutation.CreateGenre(genreData, cancellationToken);

				genres.Add(genre.Id, newGenreId);
			}

			logger.LogInformation("Seeded {GenresNumber} genres", genres.Count);

			return genres;
		}
	}
}
