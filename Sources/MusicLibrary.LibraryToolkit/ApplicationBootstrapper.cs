using System;
using CF.Library.Bootstrap;
using CF.Library.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Media;
using MusicLibrary.Dal.Extensions;
using MusicLibrary.Library;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibrary.LibraryToolkit.Seeders;
using MusicLibrary.LibraryToolkit.Settings;
using MusicLibrary.Tagger;
using MusicLibraryApi.Client;

namespace MusicLibrary.LibraryToolkit
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
