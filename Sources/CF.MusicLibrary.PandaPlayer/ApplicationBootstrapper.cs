using System;
using System.IO;
using CF.Library.Bootstrap;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.PandaPlayer.Adviser;
using CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscImages;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using CF.MusicLibrary.PandaPlayer.ViewModels.Player;
using CF.MusicLibrary.Tagger;
using CF.MusicLibrary.Universal.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.PandaPlayer
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
	public class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		private LoggerViewModel loggerViewModelInstance;

		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<SqLiteConnectionSettings>(options => configuration.Bind("database", options));
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<LastFmClientSettings>(options => configuration.Bind("lastFmClient", options));
			services.Configure<PandaPlayerSettings>(configuration.Bind);

			var dataStoragePath = configuration["dataStoragePath"];
			if (String.IsNullOrEmpty(dataStoragePath))
			{
				throw new InvalidOperationException("dataStoragePath is not configured");
			}

			services.AddTransient<IConfiguredDbConnectionFactory, SqLiteConnectionFactory>();
			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			services.AddTransient<ISongTagger, SongTagger>();
			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddSingleton<DiscLibrary>(sp => new DiscLibrary(async () =>
			{
				var library = sp.GetRequiredService<IMusicLibrary>();
				return await library.LoadDiscs();
			}));

			services.AddTransient<IApplicationViewModelHolder, ApplicationViewModelHolder>();
			services.AddTransient<INavigatedViewModelHolder, NavigatedViewModelHolder>();
			services.AddSingleton<ILibraryExplorerViewModel, LibraryExplorerViewModel>();
			services.AddSingleton<IExplorerSongListViewModel, ExplorerSongListViewModel>();
			services.AddSingleton<ISongPlaylistViewModel, PersistentSongPlaylistViewModel>();
			services.AddSingleton<IEditDiscPropertiesViewModel, EditDiscPropertiesViewModel>();
			services.AddSingleton<IEditSongPropertiesViewModel, EditSongPropertiesViewModel>();
			services.AddSingleton<IMusicPlayerViewModel, MusicPlayerViewModel>();
			services.AddSingleton<IDiscAdviserViewModel, DiscAdviserViewModel>();
			services.AddSingleton<IRateDiscViewModel, RateDiscViewModel>();
			services.AddSingleton<IDiscImageViewModel, DiscImageViewModel>();
			services.AddSingleton<IEditDiscImageViewModel, EditDiscImageViewModel>();
			services.AddSingleton<ILibraryStatisticsViewModel, LibraryStatisticsViewModel>();
			services.AddSingleton<ILoggerViewModel>(loggerViewModelInstance);
			services.AddSingleton<ApplicationViewModel>();

			services.AddTransient<ILibraryBrowser, FileSystemLibraryBrowser>();
			services.AddTransient<ITimerFacade, TimerFacade>(sp => new TimerFacade());
			services.AddTransient<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			services.AddTransient<ILastFMApiClient, LastFMApiClient>();
			services.AddTransient<IScrobbler, LastFMScrobbler>();
			services.AddTransient<ILibraryContentUpdater, LibraryContentUpdater>();
			services.AddSingleton<IViewNavigator, ViewNavigator>();
			services.AddTransient<IDiscGroupper, MyLibraryDiscGroupper>();
			services.AddTransient<IDiscGroupSorter, RankBasedDiscGroupSorter>();
			services.AddTransient<IAdviseFactorsProvider, AdviseFactorsProvider>();
			services.AddTransient<ILibraryStructurer, MyLibraryStructurer>();
			services.AddTransient<IWindowService, WpfWindowService>();
			services.AddTransient<IAudioPlayer, AudioPlayer>();
			services.AddTransient<IMediaPlayerFacade, MediaPlayerFacade>();
			services.AddTransient<ISongPlaybacksRegistrator, SongPlaybacksRegistrator>();
			services.AddTransient<IClock, SystemClock>();
			services.AddTransient<IDocumentDownloader, HttpDocumentDownloader>();
			services.AddTransient<IWebBrowser, SystemDefaultWebBrowser>();
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageFile, ImageFile>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();

			services.AddTransient<RankBasedDiscAdviser>();
			services.AddTransient<HighlyRatedSongsAdviser>();
			services.AddTransient<FavouriteArtistDiscsAdviser>(sp => new FavouriteArtistDiscsAdviser(sp.GetRequiredService<RankBasedDiscAdviser>()));

			services.AddTransient<ICompositePlaylistAdviser, CompositePlaylistAdviser>(sp => new CompositePlaylistAdviser(
				usualDiscsAdviser: sp.GetRequiredService<RankBasedDiscAdviser>(),
				highlyRatedSongsAdviser: sp.GetRequiredService<HighlyRatedSongsAdviser>(),
				favouriteArtistDiscsAdviser: sp.GetRequiredService<FavouriteArtistDiscsAdviser>(),
				memoRepository: sp.GetRequiredService<IGenericDataRepository<PlaylistAdviserMemo>>()));

			services.AddTransient<IGenericDataRepository<PlaylistData>, JsonFileGenericRepository<PlaylistData>>(
				sp => new JsonFileGenericRepository<PlaylistData>(
					sp.GetRequiredService<IFileSystemFacade>(),
					sp.GetRequiredService<ILogger<JsonFileGenericRepository<PlaylistData>>>(),
					Path.Combine(dataStoragePath, "Playlist.json")));

			services.AddTransient<IGenericDataRepository<PlaylistAdviserMemo>, JsonFileGenericRepository<PlaylistAdviserMemo>>(
				sp => new JsonFileGenericRepository<PlaylistAdviserMemo>(
					sp.GetRequiredService<IFileSystemFacade>(),
					sp.GetRequiredService<ILogger<JsonFileGenericRepository<PlaylistAdviserMemo>>>(),
					Path.Combine(dataStoragePath, "AdviserMemo.json")));
		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			base.BootstrapConfiguration(configurationBuilder, commandLineArgs);

			configurationBuilder.AddJsonFile("AppSettings.Dev.json", optional: true);
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerViewModelInstance = new LoggerViewModel();
			loggerFactory.AddProvider(new InstanceLoggerProvider(loggerViewModelInstance));
		}
	}
}
