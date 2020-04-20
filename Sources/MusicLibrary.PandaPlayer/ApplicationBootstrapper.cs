using System;
using System.Collections.Generic;
using System.IO;
using CF.Library.Bootstrap;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibrary.Common.Images;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Media;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.LastFM;
using MusicLibrary.Library;
using MusicLibrary.PandaPlayer.Adviser;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;
using MusicLibrary.PandaPlayer.ContentUpdate;
using MusicLibrary.PandaPlayer.ViewModels;
using MusicLibrary.PandaPlayer.ViewModels.DiscImages;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using MusicLibrary.PandaPlayer.ViewModels.Player;
using MusicLibrary.PandaPlayer.ViewModels.Scrobbling;
using MusicLibrary.Tagger;

namespace MusicLibrary.PandaPlayer
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		private LoggerViewModel loggerViewModelInstance;

		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<LastFmClientSettings>(options => configuration.Bind("lastFmClient", options));
			services.Configure<AdviserSettings>(options => configuration.Bind("adviser", options));
			services.Configure<FavouriteArtistsAdviserSettings>(options => configuration.Bind("adviser:favouriteArtistsAdviser", options));
			services.Configure<HighlyRatedSongsAdviserSettings>(options => configuration.Bind("adviser:highlyRatedSongsAdviser", options));
			services.Configure<GroupingSettings>(options => configuration.Bind("adviser:groupings", options));
			services.Configure<PandaPlayerSettings>(configuration.Bind);

			var dataStoragePath = configuration["dataStoragePath"];
			if (String.IsNullOrEmpty(dataStoragePath))
			{
				throw new InvalidOperationException("dataStoragePath is not configured");
			}

			services.AddLocalDbDal(settings => configuration.Bind("database", settings));

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

			services.AddTransient<ITimerFacade, TimerFacade>(sp => new TimerFacade());
			services.AddTransient<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			services.AddTransient<ILastFMApiClient, LastFMApiClient>();
			services.AddTransient<ILibraryContentUpdater, LibraryContentUpdater>();
			services.AddSingleton<IViewNavigator, ViewNavigator>();
			services.AddTransient<IDiscGroupper, LibraryDiscGroupper>();
			services.AddTransient<IDiscGroupSorter, RankBasedDiscGroupSorter>();
			services.AddTransient<IAdviseFactorsProvider, AdviseFactorsProvider>();
			services.AddTransient<ILibraryStructurer, LibraryStructurer>();
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

			services.AddTransient<LastFMScrobbler>();
			services.AddSingleton<IScrobbler, PersistentScrobbler>(sp =>
				new PersistentScrobbler(sp.GetRequiredService<LastFMScrobbler>(), sp.GetRequiredService<IScrobblesProcessor>(), sp.GetRequiredService<ILogger<PersistentScrobbler>>()));
			services.AddSingleton<IScrobblesProcessor, PersistentScrobblesProcessor>();
			services.AddTransient(typeof(Queue<>));

			services.AddTransient<RankBasedDiscAdviser>();
			services.AddTransient<HighlyRatedSongsAdviser>();
			services.AddTransient<FavouriteArtistDiscsAdviser>(sp => new FavouriteArtistDiscsAdviser(
				sp.GetRequiredService<RankBasedDiscAdviser>(),
				sp.GetRequiredService<ILogger<FavouriteArtistDiscsAdviser>>(),
				sp.GetRequiredService<IOptions<FavouriteArtistsAdviserSettings>>()));

			services.AddTransient<ICompositePlaylistAdviser, CompositePlaylistAdviser>(sp => new CompositePlaylistAdviser(
				usualDiscsAdviser: sp.GetRequiredService<RankBasedDiscAdviser>(),
				highlyRatedSongsAdviser: sp.GetRequiredService<HighlyRatedSongsAdviser>(),
				favouriteArtistDiscsAdviser: sp.GetRequiredService<FavouriteArtistDiscsAdviser>(),
				memoRepository: sp.GetRequiredService<IGenericDataRepository<PlaylistAdviserMemo>>(),
				options: sp.GetRequiredService<IOptions<AdviserSettings>>()));

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

			services.AddTransient<IScrobblesQueueRepository, ScrobblesQueueRepository>(
				sp => new ScrobblesQueueRepository(
					sp.GetRequiredService<IFileSystemFacade>(),
					sp.GetRequiredService<ILogger<ScrobblesQueueRepository>>(),
					Path.Combine(dataStoragePath, "ScrobblesQueue.json")));
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerViewModelInstance = new LoggerViewModel();

#pragma warning disable CA2000 // Dispose objects before losing scope
			loggerFactory.AddProvider(new InstanceLoggerProvider(loggerViewModelInstance));
#pragma warning restore CA2000 // Dispose objects before losing scope
		}
	}
}
