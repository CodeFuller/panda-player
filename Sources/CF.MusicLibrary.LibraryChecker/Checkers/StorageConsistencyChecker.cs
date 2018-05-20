using System;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class StorageConsistencyChecker : IStorageConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly IUriCheckScope checkScope;
		private readonly ILogger<StorageConsistencyChecker> logger;

		public StorageConsistencyChecker(IMusicLibrary musicLibrary, ILibraryInconsistencyRegistrator inconsistencyRegistrator,
			IUriCheckScope checkScope, ILogger<StorageConsistencyChecker> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.checkScope = checkScope ?? throw new ArgumentNullException(nameof(checkScope));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckStorage(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking library storage...");
			await musicLibrary.CheckStorage(library, checkScope, inconsistencyRegistrator, fixIssues);
		}

		public async Task CheckStorageChecksums(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking storage checksums...");
			await musicLibrary.CheckStorageChecksums(library, checkScope, inconsistencyRegistrator, fixIssues);
		}
	}
}
