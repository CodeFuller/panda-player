using System;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	class StorageConsistencyChecker : IStorageConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;

		private readonly ILogger<StorageConsistencyChecker> logger;

		public StorageConsistencyChecker(IMusicLibrary musicLibrary, ILibraryInconsistencyRegistrator inconsistencyRegistrator,
			ILogger<StorageConsistencyChecker> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckStorage(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking library storage ...");
			await musicLibrary.CheckStorage(library, inconsistencyRegistrator, fixIssues);
		}

		public async Task CheckStorageChecksums(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking storage checksums...");
			await musicLibrary.CheckStorageChecksums(library, inconsistencyRegistrator, fixIssues);
		}
	}
}
