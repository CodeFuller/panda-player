using System;
using CF.Library.Bootstrap;
using CF.Library.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using CF.MusicLibrary.LibraryToolkit.Seeders;
using CF.MusicLibrary.Tagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicLibraryApi.Client;

namespace CF.MusicLibrary.LibraryToolkit
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SqLiteConnectionSettings>(options => configuration.Bind("musicLibrary:database", options));
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("musicLibrary:fileSystemStorage", options));
			services.Configure<ApiConnectionSettings>(options => configuration.Bind("musicLibraryApiConnection", options));

			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IApplicationLogic, ApplicationLogic>();
			services.AddTransient<IMigrateDatabaseCommand, MigrateDatabaseCommand>();
			services.AddTransient<ISeedApiDatabaseCommand, SeedApiDatabaseCommand>();

			services.AddTransient<IGenresSeeder, GenresSeeder>();
			services.AddTransient<IArtistsSeeder, ArtistsSeeder>();
			services.AddTransient<IFoldersSeeder, FoldersSeeder>();

			services.AddTransient<IConfiguredDbConnectionFactory, SqLiteConnectionFactory>();
			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<ILibraryStructurer, LibraryStructurer>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<ISongTagger, SongTagger>();

			services.AddMusicLibraryApiClient();
		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			// We do not load configuration from command line, so that it does not conflict with utility commands.
			configurationBuilder.LoadSettings("AppSettings.json", Array.Empty<string>());
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.LoadLoggingConfiguration(configuration);
		}
	}
}
