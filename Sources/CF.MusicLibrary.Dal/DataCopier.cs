using System;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Dal.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.Dal
{
	internal class DataCopier : IDataCopier
	{
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly ILogger<DataCopier> logger;

		public DataCopier(IFileSystemFacade fileSystemFacade, ILogger<DataCopier> logger)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CopyData(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken)
		{
			CreateDatabaseSchema(targetDatabaseFileName);

			logger.LogInformation("Copying the data...");
			await MusicLibraryRepository.CopyData(GetContextSetupForDatabaseFile(sourceDatabaseFileName), GetContextSetupForDatabaseFile(targetDatabaseFileName));

			logger.LogInformation("Data was migrated successfully");
		}

		private static Action<DbContextOptionsBuilder> GetContextSetupForDatabaseFile(string databaseFileName)
		{
			var settings = new SqLiteConnectionSettings
			{
				DataSource = databaseFileName,
			};

			return settings.ToContextSetup();
		}

		private void CreateDatabaseSchema(string targetDatabaseFileName)
		{
			const string sqlFile = @"MusicLibrary.sql";
			logger.LogInformation($"Creating database schema from '{sqlFile}'...");

			var commandText = fileSystemFacade.ReadAllText(sqlFile);

			using var connection = new SqliteConnection(targetDatabaseFileName.ToConnectionString());
			connection.Open();

			using var transaction = connection.BeginTransaction();

			using var cmd = connection.CreateCommand();
			cmd.CommandText = commandText;
			cmd.ExecuteNonQuery();

			transaction.Commit();
		}
	}
}
