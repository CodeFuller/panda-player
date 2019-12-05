using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryToolkit
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

		public async Task Execute(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken)
		{
			const string sqlFile = @"MusicLibrary.sql";

			var sourceDBConnectionString = BuildConnectionString(sourceDatabaseFileName);
			var targetDBConnectionString = BuildConnectionString(targetDatabaseFileName);

			// Checking that target database file does not exist.
			if (fileSystemFacade.FileExists(targetDatabaseFileName))
			{
				logger.LogError($"Target database file should not exist: '{targetDatabaseFileName}'");
				return;
			}

			logger.LogInformation($"Creating database schema from '{sqlFile}'...");
			CreateDatabaseSchema(sqlFile, targetDBConnectionString);

			logger.LogInformation("Copying the data...");
			using (var sourceConnection = new SQLiteConnection(sourceDBConnectionString))
			using (var targetConnection = new SQLiteConnection(targetDBConnectionString))
			{
				await MusicLibraryRepositoryEF.CopyData(sourceConnection, targetConnection);
			}

			logger.LogInformation("Data was migrated successfully");
		}

		private static string BuildConnectionString(string databaseFileName)
		{
			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
			{
				DataSource = databaseFileName,
				ForeignKeys = true,
			};

			return builder.ConnectionString;
		}

		private static void CreateDatabaseSchema(string sqlScriptFileName, string targetDBConnectionString)
		{
			var commandText = File.ReadAllText(sqlScriptFileName);

			using (IDbConnection connection = new SQLiteConnection(targetDBConnectionString))
			{
				connection.ConnectionString = targetDBConnectionString;
				connection.Open();

				using (IDbTransaction transaction = connection.BeginTransaction())
				using (IDbCommand cmd = connection.CreateCommand())
				{
					cmd.CommandText = commandText;
					cmd.ExecuteNonQuery();
					transaction.Commit();
				}
			}
		}
	}
}
