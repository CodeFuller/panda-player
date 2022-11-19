using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Services.Diagnostic;
using PandaPlayer.Services.Diagnostic.Inconsistencies;
using PandaPlayer.Services.Diagnostic.Interfaces;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class DiagnosticService : IDiagnosticService
	{
		private readonly IDiscConsistencyChecker discConsistencyChecker;

		private readonly IStorageRepository storageRepository;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public DiagnosticService(IDiscConsistencyChecker discConsistencyChecker, IStorageRepository storageRepository)
		{
			this.discConsistencyChecker = discConsistencyChecker ?? throw new ArgumentNullException(nameof(discConsistencyChecker));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
		}

		public async Task CheckLibrary(LibraryCheckFlags checkFlags, IOperationProgress progress, Action<LibraryInconsistency> inconsistenciesHandler, CancellationToken cancellationToken)
		{
			var activeDiscs = DiscLibrary.Discs.Where(d => !d.IsDeleted).ToList();

			if (checkFlags.HasFlag(LibraryCheckFlags.CheckDiscsConsistency))
			{
				await discConsistencyChecker.CheckDiscsConsistency(activeDiscs, inconsistenciesHandler, cancellationToken);
			}

			if (checkFlags.HasFlag(LibraryCheckFlags.CheckStorageConsistency))
			{
				var activeFolders = DiscLibrary.Folders.Where(f => !f.IsDeleted);

				await storageRepository.CheckStorage(checkFlags, activeFolders, activeDiscs, progress, inconsistenciesHandler, cancellationToken);
			}
		}
	}
}
