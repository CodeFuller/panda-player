using System;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryToolkit
{
	public class SeedApiDatabaseCommand : ISeedApiDatabaseCommand
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly IGenresSeeder genresSeeder;

		private readonly IArtistsSeeder artistsSeeder;

		private readonly IFoldersSeeder foldersSeeder;

		private readonly ILogger<SeedApiDatabaseCommand> logger;

		public SeedApiDatabaseCommand(IMusicLibrary musicLibrary, IGenresSeeder genresSeeder, IArtistsSeeder artistsSeeder,
			IFoldersSeeder foldersSeeder, ILogger<SeedApiDatabaseCommand> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.genresSeeder = genresSeeder ?? throw new ArgumentNullException(nameof(genresSeeder));
			this.artistsSeeder = artistsSeeder ?? throw new ArgumentNullException(nameof(artistsSeeder));
			this.foldersSeeder = foldersSeeder ?? throw new ArgumentNullException(nameof(foldersSeeder));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			var genres = await genresSeeder.SeedGenres(discLibrary, cancellationToken);
			var artists = await artistsSeeder.SeedArtists(discLibrary, cancellationToken);
			var folders = await foldersSeeder.SeedFolders(discLibrary, cancellationToken);
		}
	}
}
