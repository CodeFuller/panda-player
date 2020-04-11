using System;
using CF.Library.Bootstrap;
using CF.Library.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Dal.Extensions;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.LibraryToolkit.Interfaces;
using CF.MusicLibrary.LibraryToolkit.Seeders;
using CF.MusicLibrary.LibraryToolkit.Settings;
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
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("musicLibrary:fileSystemStorage", options));
			services.Configure<DiscsSeederSettings>(options => configuration.Bind("seeders:discsSeeder", options));
			services.Configure<SongsSeederSettings>(options => configuration.Bind("seeders:songsSeeder", options));

			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IApplicationLogic, ApplicationLogic>();
			services.AddTransient<IMigrateDatabaseCommand, MigrateDatabaseCommand>();
			services.AddTransient<ISeedApiDatabaseCommand, SeedApiDatabaseCommand>();

			services.AddTransient<IGenresSeeder, GenresSeeder>();
			services.AddTransient<IArtistsSeeder, ArtistsSeeder>();
			services.AddTransient<IFoldersSeeder, FoldersSeeder>();
			services.AddTransient<IDiscsSeeder, DiscsSeeder>();
			services.AddTransient<ISongsSeeder, SongsSeeder>();
			services.AddTransient<IPlaybacksSeeder, PlaybacksSeeder>();

			services.AddDal(settings => configuration.Bind("musicLibrary:database", settings));

			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<ILibraryStructurer, LibraryStructurer>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<IMusicLibraryReader, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<ISongTagger, SongTagger>();

			services.AddMusicLibraryApiClient(settings => configuration.Bind("musicLibraryApiConnection", settings));
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
