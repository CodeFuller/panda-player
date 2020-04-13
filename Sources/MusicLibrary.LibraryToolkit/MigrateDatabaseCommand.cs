using System;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Logging;
using MusicLibrary.Dal;
using MusicLibrary.LibraryToolkit.Interfaces;

namespace MusicLibrary.LibraryToolkit
{
	public class MigrateDatabaseCommand : IMigrateDatabaseCommand
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly IDataCopier dataCopier;

		private readonly ILogger<MigrateDatabaseCommand> logger;

		public MigrateDatabaseCommand(IDataCopier dataCopier, IFileSystemFacade fileSystemFacade, ILogger<MigrateDatabaseCommand> logger)
		{
			this.dataCopier = dataCopier ?? throw new ArgumentNullException(nameof(dataCopier));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Execute(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken)
		{
			// Checking that target database file does not exist.
			if (fileSystemFacade.FileExists(targetDatabaseFileName))
			{
				logger.LogError($"Target database file should not exist: '{targetDatabaseFileName}'");
				return;
			}

			await dataCopier.CopyData(sourceDatabaseFileName, targetDatabaseFileName, cancellationToken);
		}
	}
}
