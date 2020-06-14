using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Services.Diagnostic;
using MusicLibrary.Services.Diagnostic.Inconsistencies;
using MusicLibrary.Services.Diagnostic.Interfaces;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
{
	internal class DiagnosticService : IDiagnosticService
	{
		private readonly IFoldersService foldersService;

		private readonly IDiscsService discsService;

		private readonly IDiscConsistencyChecker discConsistencyChecker;

		private readonly ITagsConsistencyChecker tagsConsistencyChecker;

		private readonly IStorageRepository storageRepository;

		public DiagnosticService(IFoldersService foldersService, IDiscsService discsService, IDiscConsistencyChecker discConsistencyChecker,
			IStorageRepository storageRepository, ITagsConsistencyChecker tagsConsistencyChecker)
		{
			this.foldersService = foldersService ?? throw new ArgumentNullException(nameof(foldersService));
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.discConsistencyChecker = discConsistencyChecker ?? throw new ArgumentNullException(nameof(discConsistencyChecker));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.tagsConsistencyChecker = tagsConsistencyChecker ?? throw new ArgumentNullException(nameof(tagsConsistencyChecker));
		}

		public async Task CheckLibrary(LibraryCheckFlags checkFlags, IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			var discs = await discsService.GetAllDiscs(cancellationToken);
			var activeDiscs = discs.Where(d => !d.IsDeleted).ToList();

			if (checkFlags.HasFlag(LibraryCheckFlags.CheckDiscsConsistency))
			{
				await discConsistencyChecker.CheckDiscsConsistency(activeDiscs, inconsistenciesHandler, cancellationToken);
			}

			if (checkFlags.HasFlag(LibraryCheckFlags.CheckStorageConsistency))
			{
				var folders = await foldersService.GetAllFolders(cancellationToken);
				var activeFolders = folders.Where(f => !f.IsDeleted);

				await storageRepository.CheckStorage(checkFlags, activeFolders, activeDiscs, progress, inconsistenciesHandler, cancellationToken);
			}

			if (checkFlags.HasFlag(LibraryCheckFlags.CheckSongTagsConsistency))
			{
				var activeSongs = activeDiscs.SelectMany(disc => disc.ActiveSongs);
				await tagsConsistencyChecker.CheckTagsConsistency(activeSongs, inconsistenciesHandler, cancellationToken);
			}
		}
	}
}
