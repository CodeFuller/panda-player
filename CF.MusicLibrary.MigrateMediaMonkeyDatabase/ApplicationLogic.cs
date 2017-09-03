using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using CF.Library.Core.Bootstrap;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Dal;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.MigrateMediaMonkeyDatabase
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is instantiated by DI Container.")]
	internal class ApplicationLogic : IApplicationLogic
	{
		private readonly IMusicLibraryRepository musicLibraryRepository;
		private readonly ConnectionStringSettings targetDBSettings;

		public ApplicationLogic(IMusicLibraryRepository musicLibraryRepository, ConnectionStringSettings targetDBSettings)
		{
			if (musicLibraryRepository == null)
			{
				throw new ArgumentNullException(nameof(musicLibraryRepository));
			}
			if (targetDBSettings == null)
			{
				throw new ArgumentNullException(nameof(targetDBSettings));
			}

			this.musicLibraryRepository = musicLibraryRepository;
			this.targetDBSettings = targetDBSettings;
		}

		public int Run(string[] args)
		{
			Console.WriteLine("Loading migrated library...");
			DiscLibrary library = musicLibraryRepository.GetDiscLibraryAsync().Result;

			if (targetDBSettings.ProviderName == "System.Data.SQLite")
			{
				ClearExistingData();
			}

			Console.WriteLine("Adding discs...");
			using (var ctx = new MusicLibraryEntities())
			{
				ctx.Configuration.AutoDetectChangesEnabled = false;

				foreach (var disc in library)
				{
					ctx.Discs.Add(disc);
				}

				Console.WriteLine("Saving changes...");
				ctx.SaveChanges();

				Console.WriteLine("Finished successfully!");
			};

			return 0;
		}

		private void ClearExistingData()
		{
			Console.WriteLine("Deleting existing data");

			using (IDbConnection connection = DbProviderFactories.GetFactory(targetDBSettings.ProviderName).CreateConnection())
			{
				connection.ConnectionString = targetDBSettings.ConnectionString;
				connection.Open();

				using (IDbTransaction transaction = connection.BeginTransaction())
				{
					ClearTable(connection, "Playbacks");
					ClearTable(connection, "Songs");
					ClearTable(connection, "Genres");
					ClearTable(connection, "Artists");
					ClearTable(connection, "Discs");
					transaction.Commit();
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities", Justification = "Command text doesn't contain user input")]
		private static void ClearTable(IDbConnection connection, string tableName)
		{
			Console.WriteLine(Current($"Deleting data from table '{tableName}'..."));

			using (IDbCommand cmd = connection.CreateCommand())
			{
				cmd.CommandText = Invariant($"DELETE FROM {tableName};");
				cmd.ExecuteNonQuery();
			}
		}
	}
}
