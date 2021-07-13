using System;
using System.Collections.Generic;
using CodeFuller.Library.Bootstrap;
using CodeFuller.Library.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PandaPlayer.Adviser.Extensions;
using PandaPlayer.Core.Facades;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.DiscAdder.Extensions;
using PandaPlayer.Facades;
using PandaPlayer.LastFM.Extensions;
using PandaPlayer.Services.Extensions;
using PandaPlayer.Settings;
using PandaPlayer.Shared;
using PandaPlayer.ViewModels;
using PandaPlayer.ViewModels.DiscImages;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.PersistentPlaylist;
using PandaPlayer.ViewModels.Player;
using PandaPlayer.ViewModels.Scrobbling;

namespace PandaPlayer
{
	internal class ApplicationBootstrapper : BasicApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<PandaPlayerSettings>(configuration.Bind);

			services.AddLocalDbDal(settings => configuration.Bind("localDb:dataStorage", settings));
			services.AddMusicDbContext(configuration.GetConnectionString("pandaPlayerDb"));
			services.AddPandaPlayerServices();
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
			services.AddSingleton<IDiscSongListViewModel, DiscSongListViewModel>();
			services.AddSingleton<IPlaylistViewModel, PersistentPlaylistViewModel>();
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
