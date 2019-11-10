using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using NDesk.Options;
using static System.FormattableString;

namespace CF.MusicLibrary.LibraryToolkit
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly IMigrateDatabaseCommand migrateDatabaseCommand;
		private readonly ISeedApiDatabaseCommand seedApiDatabaseCommand;
		private readonly IFileSystemFacade fileSystemFacade;

		public ApplicationLogic(IMigrateDatabaseCommand migrateDatabaseCommand, ISeedApiDatabaseCommand seedApiDatabaseCommand, IFileSystemFacade fileSystemFacade)
		{
			this.migrateDatabaseCommand = migrateDatabaseCommand ?? throw new ArgumentNullException(nameof(migrateDatabaseCommand));
			this.seedApiDatabaseCommand = seedApiDatabaseCommand ?? throw new ArgumentNullException(nameof(seedApiDatabaseCommand));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
		}

		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			LaunchCommand command = LaunchCommand.ShowHelp;

			var optionSet = new OptionSet
			{
				{ "migrate-database", s => command = LaunchCommand.MigrateDatabase },
				{ "seed-api-database", s => command = LaunchCommand.SeedApiDatabase },
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

					await migrateDatabaseCommand.Execute(restArgs[0], restArgs[1], cancellationToken);
					break;

				case LaunchCommand.SeedApiDatabase:
					if (restArgs.Count != 2)
					{
						ShowHelp();
						break;
					}

					await seedApiDatabaseCommand.Execute(restArgs[0], new Uri(restArgs[1], UriKind.Absolute), cancellationToken);
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
			Console.Error.WriteLine("  --migrate-database   <source db file>  <target db file>");
			Console.Error.WriteLine("      Creates database schema by included 'MusicLibrary.sql' and copies data from source database.");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --seed-api-database  <source db file>  <Music Library API base URL (e.g. http://localhost/MusicLibraryApi/)>");
			Console.Error.WriteLine("      Copies the data to Music Library API.");
			Console.Error.WriteLine();
		}
	}
}
