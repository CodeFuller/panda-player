using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.LibraryToolkit.Interfaces;

namespace MusicLibrary.LibraryToolkit
{
	public class SeedApiDatabaseCommand : ISeedApiDatabaseCommand
	{
		private readonly IGenresSeeder genresSeeder;

		private readonly IArtistsSeeder artistsSeeder;

		private readonly IFoldersSeeder foldersSeeder;

		private readonly IDiscsSeeder discsSeeder;

		private readonly ISongsSeeder songsSeeder;

		private readonly IPlaybacksSeeder playbacksSeeder;

		private readonly ILogger<SeedApiDatabaseCommand> logger;

		public SeedApiDatabaseCommand(IGenresSeeder genresSeeder, IArtistsSeeder artistsSeeder,
			IFoldersSeeder foldersSeeder, IDiscsSeeder discsSeeder, ISongsSeeder songsSeeder, IPlaybacksSeeder playbacksSeeder, ILogger<SeedApiDatabaseCommand> logger)
		{
			this.genresSeeder = genresSeeder ?? throw new ArgumentNullException(nameof(genresSeeder));
			this.artistsSeeder = artistsSeeder ?? throw new ArgumentNullException(nameof(artistsSeeder));
			this.foldersSeeder = foldersSeeder ?? throw new ArgumentNullException(nameof(foldersSeeder));
			this.discsSeeder = discsSeeder ?? throw new ArgumentNullException(nameof(discsSeeder));
			this.songsSeeder = songsSeeder ?? throw new ArgumentNullException(nameof(songsSeeder));
			this.playbacksSeeder = playbacksSeeder ?? throw new ArgumentNullException(nameof(playbacksSeeder));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Execute(CancellationToken cancellationToken)
		{
			var genres = await genresSeeder.SeedGenres(cancellationToken);
			var artists = await artistsSeeder.SeedArtists(cancellationToken);
			var folders = await foldersSeeder.SeedFolders(cancellationToken);
			var discs = await discsSeeder.SeedDiscs(folders, cancellationToken);
			var songs = await songsSeeder.SeedSongs(discs, artists, genres, cancellationToken);
			await playbacksSeeder.SeedPlaybacks(songs, cancellationToken);
		}
	}
}
