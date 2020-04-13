using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Interfaces;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public class LibraryConsistencyChecker : ILibraryConsistencyChecker
	{
		private readonly IDiscConsistencyChecker discConsistencyChecker;
		private readonly IStorageConsistencyChecker storageConsistencyChecker;
		private readonly ITagDataConsistencyChecker tagDataChecker;
		private readonly ILastFMConsistencyChecker lastFmConsistencyChecker;
		private readonly IDiscImagesConsistencyChecker discImagesConsistencyChecker;

		private readonly IMusicLibrary musicLibrary;
		private readonly ILogger<LibraryConsistencyChecker> logger;

		public LibraryConsistencyChecker(IDiscConsistencyChecker discConsistencyChecker, IStorageConsistencyChecker storageConsistencyChecker,
			ITagDataConsistencyChecker tagDataChecker, ILastFMConsistencyChecker lastFmConsistencyChecker,
			IDiscImagesConsistencyChecker discImagesConsistencyChecker, IMusicLibrary musicLibrary,
			ILogger<LibraryConsistencyChecker> logger)
		{
			this.discConsistencyChecker = discConsistencyChecker ?? throw new ArgumentNullException(nameof(discConsistencyChecker));
			this.storageConsistencyChecker = storageConsistencyChecker ?? throw new ArgumentNullException(nameof(storageConsistencyChecker));
			this.tagDataChecker = tagDataChecker ?? throw new ArgumentNullException(nameof(tagDataChecker));
			this.lastFmConsistencyChecker = lastFmConsistencyChecker ?? throw new ArgumentNullException(nameof(lastFmConsistencyChecker));
			this.discImagesConsistencyChecker = discImagesConsistencyChecker ?? throw new ArgumentNullException(nameof(discImagesConsistencyChecker));
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckLibrary(LibraryCheckFlags checkFlags, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			if ((checkFlags & LibraryCheckFlags.CheckDiscsConsistency) != 0)
			{
				await discConsistencyChecker.CheckDiscsConsistency(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckLibraryStorage) != 0)
			{
				await storageConsistencyChecker.CheckStorage(discLibrary, fixIssues, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckChecksums) != 0)
			{
				await storageConsistencyChecker.CheckStorageChecksums(discLibrary, fixIssues, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckTagData) != 0)
			{
				await tagDataChecker.CheckTagData(discLibrary.Songs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckImages) != 0)
			{
				await discImagesConsistencyChecker.CheckDiscImagesConsistency(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckArtistsOnLastFm) != 0)
			{
				await lastFmConsistencyChecker.CheckArtists(discLibrary, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckAlbumsOnLastFm) != 0)
			{
				await lastFmConsistencyChecker.CheckAlbums(discLibrary.Discs, cancellationToken);
			}

			if ((checkFlags & LibraryCheckFlags.CheckSongsOnLastFm) != 0)
			{
				await lastFmConsistencyChecker.CheckSongs(discLibrary.Songs, cancellationToken);
			}

			logger.LogInformation("Library check has finished");
		}

		public async Task UnifyTags(CancellationToken cancellationToken)
		{
			logger.LogInformation("Loading library content...");
			var discLibrary = await musicLibrary.LoadLibrary();

			await tagDataChecker.UnifyTags(discLibrary.Songs, cancellationToken);
		}
	}
}
