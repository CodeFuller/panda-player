using CF.Library.Bootstrap;
using CF.Library.Core.Facades;
using CF.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicLibrary.Common.Images;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Media;
using MusicLibrary.Dal.Extensions;
using MusicLibrary.LastFM;
using MusicLibrary.Library;
using MusicLibrary.LibraryChecker.Checkers;
using MusicLibrary.LibraryChecker.Registrators;
using MusicLibrary.Tagger;

namespace MusicLibrary.LibraryChecker
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<CheckingSettings>(options => configuration.Bind("checkingSettings", options));
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<LastFmClientSettings>(options => configuration.Bind("lastFmClient", options));
			services.Configure<DiscToAlbumMappingSettings>(options => configuration.Bind("discToAlbumMappings", options));

			services.AddDal(settings => configuration.Bind("database", settings));

			services.AddTransient<IApplicationLogic, ApplicationLogic>();
			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<ILibraryStructurer, LibraryStructurer>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<ISongTagger, SongTagger>();
			services.AddTransient<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			services.AddTransient<ILastFMApiClient, LastFMApiClient>();
			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();
			services.AddSingleton<IDiscTitleToAlbumMapper, DiscTitleToAlbumMapper>();

			// CheckScope is registered as Singleton because the scope is filled from command line in ApplicationLogic and should be shared by all checkers.
			// The same instance is registered for both ICheckScope and IUriCheckScope.
			services.AddSingleton<IUriCheckScope, CheckScope>();
			services.AddSingleton<ICheckScope>(x => x.GetService<IUriCheckScope>());

			services.AddTransient<IDiscConsistencyChecker, DiscConsistencyChecker>();
			services.AddTransient<IStorageConsistencyChecker, StorageConsistencyChecker>();
			services.AddTransient<ITagDataConsistencyChecker, TagDataConsistencyChecker>();
			services.AddTransient<ILastFMConsistencyChecker, LastFMConsistencyChecker>();
			services.AddTransient<IDiscImagesConsistencyChecker, DiscImagesConsistencyChecker>();
			services.AddTransient<ILibraryConsistencyChecker, LibraryConsistencyChecker>();

			services.AddTransient<ILibraryInconsistencyFilter, LibraryInconsistencyFilter>();
			services.AddTransient<InconsistencyRegistratorToLog>();
			services.AddTransient<ILibraryInconsistencyRegistrator, InconsistencyRegistratorWithFilter>(
				sp => new InconsistencyRegistratorWithFilter(
					sp.GetRequiredService<InconsistencyRegistratorToLog>(),
					sp.GetRequiredService<ILibraryInconsistencyFilter>()));
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.LoadLoggingConfiguration(configuration);
		}
	}
}
