using System.Collections.Generic;
using CF.Library.Bootstrap;
using CF.Library.Core;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.LastFM.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Settings;
using MusicLibrary.PandaPlayer.ViewModels;
using MusicLibrary.PandaPlayer.ViewModels.DiscImages;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using MusicLibrary.PandaPlayer.ViewModels.Player;
using MusicLibrary.PandaPlayer.ViewModels.Scrobbling;
using MusicLibrary.Services.Extensions;
using MusicLibrary.Shared;

namespace MusicLibrary.PandaPlayer
{
	internal class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		private LoggerViewModel loggerViewModelInstance;

		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<PandaPlayerSettings>(configuration.Bind);

			services.AddLocalDbDal(settings => configuration.Bind("localDb:dataStorage", settings));
			services.AddMusicLibraryDbContext(configuration.GetConnectionString("musicLibraryDb"));
			services.AddMusicLibraryServices();
			services.AddDiscTitleToAlbumMapper(settings => configuration.Bind("discToAlbumMappings", settings));

			services.AddDiscAdder(settings => configuration.Bind("discAdder", settings));

			RegisterViewModels(services);
			services.AddImages();

			services.AddTransient<ITimerFacade, TimerFacade>(sp => new TimerFacade());
			services.AddSingleton<IViewNavigator, ViewNavigator>();
			services.AddTransient<IWindowService, WpfWindowService>();
			services.AddTransient<IAudioPlayer, AudioPlayer>();
			services.AddTransient<IMediaPlayerFacade, MediaPlayerFacade>();
			services.AddTransient<ISongPlaybacksRegistrator, SongPlaybacksRegistrator>();
			services.AddTransient<IClock, SystemClock>();
			services.AddTransient<IDocumentDownloader, HttpDocumentDownloader>();
			services.AddTransient<IWebBrowser, SystemDefaultWebBrowser>();

			services.AddLastFmScrobbler(settings => configuration.Bind("lastFmClient", settings));
			services.WrapScrobbler<PersistentScrobbler>(ServiceLifetime.Singleton);
			services.AddSingleton<IScrobblesProcessor, PersistentScrobblesProcessor>();
			services.AddTransient(typeof(Queue<>));

			services.AddPlaylistAdviser(settings =>
			{
				configuration.Bind("adviser", settings);
				configuration.Bind("adviser:favoriteArtistsAdviser", settings.FavoriteArtistsAdviser);
				configuration.Bind("adviser:highlyRatedSongsAdviser", settings.HighlyRatedSongsAdviser);
			});
		}

		private void RegisterViewModels(IServiceCollection services)
		{
			services.AddTransient<IApplicationViewModelHolder, ApplicationViewModelHolder>();
			services.AddTransient<INavigatedViewModelHolder, NavigatedViewModelHolder>();
			services.AddSingleton<ILibraryExplorerViewModel, LibraryExplorerViewModel>();
			services.AddSingleton<IExplorerSongListViewModel, ExplorerSongListViewModel>();
			services.AddSingleton<ISongPlaylistViewModel, PersistentSongPlaylistViewModel>();
			services.AddSingleton<IEditDiscPropertiesViewModel, EditDiscPropertiesViewModel>();
			services.AddSingleton<IEditSongPropertiesViewModel, EditSongPropertiesViewModel>();
			services.AddSingleton<IMusicPlayerViewModel, MusicPlayerViewModel>();
			services.AddSingleton<IDiscAdviserViewModel, DiscAdviserViewModel>();
			services.AddSingleton<IRateSongsViewModel, RateSongsViewModel>();
			services.AddSingleton<IDiscImageViewModel, DiscImageViewModel>();
			services.AddSingleton<IEditDiscImageViewModel, EditDiscImageViewModel>();
			services.AddSingleton<ILibraryCheckerViewModel, LibraryCheckerViewModel>();
			services.AddSingleton<ILibraryStatisticsViewModel, LibraryStatisticsViewModel>();
			services.AddSingleton<ILoggerViewModel>(loggerViewModelInstance);
			services.AddSingleton<ApplicationViewModel>();
		}

		protected override ILoggerFactory BootstrapLogging(IConfiguration configuration)
		{
			var settings = new LoggingSettings();
			configuration.Bind("logging", settings);

			var loggerFactory = LoggerFactory.Create(loggingBuilder =>
			{
				loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore", settings.DatabaseOperationsLogLevel);
			});

			loggerViewModelInstance = new LoggerViewModel(Options.Create(settings));

#pragma warning disable CA2000 // Dispose objects before losing scope
			loggerFactory.AddProvider(new InstanceLoggerProvider(loggerViewModelInstance));
#pragma warning restore CA2000 // Dispose objects before losing scope

			return loggerFactory;
		}
	}
}
