using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Dal;
using Microsoft.Extensions.Logging;
using NDesk.Options;
using static System.FormattableString;

namespace CF.MusicLibrary.LibraryToolkit
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated by DI Container.")]
	internal class ApplicationLogic : IApplicationLogic
	{
		private readonly IFileSystemFacade fileSystemFacade;
		private readonly ILogger<ApplicationLogic> logger;

		public ApplicationLogic(IFileSystemFacade fileSystemFacade, ILogger<ApplicationLogic> logger)
		{
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			LaunchCommand command = LaunchCommand.ShowHelp;

			var optionSet = new OptionSet
			{
				{ "migrate-database", s => command = LaunchCommand.MigrateDatabase },
			};
			var restArgs = optionSet.Parse(args);

			switch (command)
			{
				case LaunchCommand.ShowHelp:
					ShowHelp();
					return 1;

				case LaunchCommand.MigrateDatabase:
					if (restArgs.Count != 2)
					{
						ShowHelp();
						break;
					}
					await MigrateDatabase(restArgs[0], restArgs[1]);
					break;

				default:
					throw new UnexpectedEnumValueException(command);
			}

			return 0;
		}

		private void ShowHelp()
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine(Invariant($"Usage: {Path.GetFileName(fileSystemFacade.GetProcessExecutableFileName())} <command> [command options]"));
			Console.Error.WriteLine("Supported commands:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --migrate-database  <source db file>  <target db file>");
			Console.Error.WriteLine("      Creates database schema by included 'MusicLibrary.sql' and copies data from source database.");
			Console.Error.WriteLine();
		}

		private async Task MigrateDatabase(string sourceDatabaseFileName, string targetDatabaseFileName)
		{
			const string sqlFile = @"MusicLibrary.sql";

			var sourceDBConnectionString = BuildConnectionString(sourceDatabaseFileName);
			var targetDBConnectionString = BuildConnectionString(targetDatabaseFileName);

			//	Checking that target database is empty
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Application logic requires execution of SQL commands from the file")]
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

		private static string BuildConnectionString(string databaseFileName)
		{
			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
			{
				DataSource = databaseFileName,
				ForeignKeys = true
			};

			return builder.ConnectionString;
		}
	}
}
