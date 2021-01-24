using System;
using System.Collections.Generic;
using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Facades;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.LastFM.Extensions;
using MusicLibrary.PandaPlayer.Adviser.Extensions;
using MusicLibrary.PandaPlayer.Facades;
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
	internal class ApplicationBootstrapper : BasicApplicationBootstrapper<ApplicationViewModel>
	{
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

			services.AddTransient<ITimerFacade, TimerFacade>(_ => new TimerFacade());
			services.AddSingleton<IViewNavigator, ViewNavigator>();
			services.AddWpfWindowService();
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

		private static void RegisterViewModels(IServiceCollection services)
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

			services.AddSingleton<LoggerViewModel>();
			services.AddSingleton<ILoggerViewModel, LoggerViewModel>(sp => sp.GetRequiredService<LoggerViewModel>());
			services.AddSingleton<InstanceLoggerProvider>(
				sp => ActivatorUtilities.CreateInstance<InstanceLoggerProvider>(sp, sp.GetRequiredService<LoggerViewModel>()));

			services.AddSingleton<ApplicationViewModel>();
		}

		protected override ILoggerFactory BootstrapLogging(IServiceProvider serviceProvider, IConfiguration configuration)
		{
			var settings = configuration
				.GetSection("logging")
				.Get<LoggingSettings>();

			var loggerFactory = LoggerFactory.Create(loggingBuilder =>
			{
				loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore", settings.DatabaseOperationsLogLevel);
			});

			loggerFactory.AddProvider(serviceProvider.GetRequiredService<InstanceLoggerProvider>());

			return loggerFactory;
		}
	}
}
