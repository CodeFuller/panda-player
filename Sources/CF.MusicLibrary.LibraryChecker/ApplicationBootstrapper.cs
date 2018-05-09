using CF.Library.Bootstrap;
using CF.Library.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.LibraryChecker.Checkers;
using CF.MusicLibrary.LibraryChecker.Registrators;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.Tagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryChecker
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CheckingSettings>(options => configuration.Bind("checkingSettings", options));
			services.Configure<SqLiteConnectionSettings>(options => configuration.Bind("database", options));
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<LastFmClientSettings>(options => configuration.Bind("lastFmClient", options));

			services.AddTransient<IApplicationLogic, ApplicationLogic>();
			services.AddTransient<IConfiguredDbConnectionFactory, SqLiteConnectionFactory>();
			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<ILibraryStructurer, MyLibraryStructurer>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<ISongTagger, SongTagger>();
			services.AddTransient<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			services.AddTransient<ILastFMApiClient, LastFMApiClient>();
			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();

			services.AddTransient<IDiscConsistencyChecker, DiscConsistencyChecker>();
			services.AddTransient<IStorageConsistencyChecker, StorageConsistencyChecker>();
			services.AddTransient<ITagDataConsistencyChecker, TagDataConsistencyChecker>();
			services.AddTransient<ILastFMConsistencyChecker, LastFMConsistencyChecker>();
			services.AddTransient<IDiscImagesConsistencyChecker, DiscImagesConsistencyChecker>();

			services.AddTransient<ILibraryInconsistencyFilter, LibraryInconsistencyFilter>();
			services.AddTransient<InconsistencyRegistratorToLog>();
			services.AddTransient<ILibraryInconsistencyRegistrator, InconsistencyRegistratorWithFilter>(
				sp => new InconsistencyRegistratorWithFilter(
					sp.GetRequiredService<InconsistencyRegistratorToLog>(),
					sp.GetRequiredService<ILibraryInconsistencyFilter>()));
		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			// We do not load configuration from command line, so that it does not conflict with utility commands.
			configurationBuilder.LoadSettings("AppSettings.json", new string[0])
				.AddJsonFile("AppSettings.Dev.json", optional: true);
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.LoadLoggingConfiguration(configuration);
		}
	}
}
