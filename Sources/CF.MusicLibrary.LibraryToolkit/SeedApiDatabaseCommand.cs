using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;
using MusicLibraryApi.Client.Contracts.Artists;
using MusicLibraryApi.Client.Contracts.Genres;
using MusicLibraryApi.Client.Interfaces;

namespace CF.MusicLibrary.LibraryToolkit
{
	public class SeedApiDatabaseCommand : ISeedApiDatabaseCommand
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly IGenresMutation genresMutation;

		private readonly IArtistsMutation artistsMutation;

		private readonly ILogger<SeedApiDatabaseCommand> logger;

		public SeedApiDatabaseCommand(IMusicLibrary musicLibrary, IGenresMutation genresMutation, IArtistsMutation artistsMutation, ILogger<SeedApiDatabaseCommand> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.genresMutation = genresMutation ?? throw new ArgumentNullException(nameof(genresMutation));
			this.artistsMutation = artistsMutation ?? throw new ArgumentNullException(nameof(artistsMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			var genres = await SeedGenres(discLibrary, cancellationToken);
			var artists = await SeedArtists(discLibrary, cancellationToken);
		}

		private async Task<IDictionary<string, int>> SeedGenres(DiscLibrary discLibrary, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding genres ...");

			var genres = new Dictionary<string, int>();
			foreach (var genre in discLibrary.Genres.OrderBy(g => g.Name))
			{
				var genreData = new InputGenreData(genre.Name);
				var newGenreId = await genresMutation.CreateGenre(genreData, cancellationToken);

				genres.Add(genre.Name, newGenreId);
			}

			logger.LogInformation("Seeded {GenresNumber} genres", genres.Count);

			return genres;
		}

		private async Task<IDictionary<string, int>> SeedArtists(DiscLibrary discLibrary, CancellationToken cancellationToken)
		{
			logger.LogInformation("Seeding artists ...");

			var artists = new Dictionary<string, int>();
			foreach (var artist in discLibrary.AllArtists.OrderBy(g => g.Name))
			{
				var artistData = new InputArtistData(artist.Name);
				var newArtistId = await artistsMutation.CreateArtist(artistData, cancellationToken);

				artists.Add(artist.Name, newArtistId);
			}

			logger.LogInformation("Seeded {ArtistsNumber} artists", artists.Count);

			return artists;
		}
	}
}
