using System;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Logging;
using MusicLibrary.LibraryToolkit.Interfaces;

namespace MusicLibrary.LibraryToolkit
{
	public class MigrateDatabaseCommand : IMigrateDatabaseCommand
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly ILogger<MigrateDatabaseCommand> logger;

		public MigrateDatabaseCommand(IFileSystemFacade fileSystemFacade, ILogger<MigrateDatabaseCommand> logger)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task Execute(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken)
		{
			// TODO: Restore this functionality or delete it completely.
			throw new NotImplementedException();
/*
			// Checking that target database file does not exist.
			if (fileSystemFacade.FileExists(targetDatabaseFileName))
			{
				logger.LogError($"Target database file should not exist: '{targetDatabaseFileName}'");
			}
*/
		}
	}
}
