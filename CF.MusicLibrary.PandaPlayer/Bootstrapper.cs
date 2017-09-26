﻿using System.IO;
using CF.Library.Core;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Core.Logging;
using CF.Library.Unity;
using CF.Library.Wpf;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.DiscAdviser;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using CF.MusicLibrary.PandaPlayer.ViewModels.Player;
using CF.MusicLibrary.Tagger;
using CF.MusicLibrary.Universal.DiscArt;
using CF.MusicLibrary.Universal.Interfaces;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.PandaPlayer
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
	internal class Bootstrapper : UnityBootstrapper<ApplicationViewModel>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by DI Container.")]
		protected override void RegisterDependencies()
		{
			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");
			string lastFMApiKey = AppSettings.GetRequiredValue<string>("LastFMApiKey");

			string lastFMSharedSecret = AppSettings.GetPrivateRequiredValue<string>("LastFMSharedSecret");
			string lastFMSessionKey = AppSettings.GetPrivateRequiredValue<string>("LastFMSessionKey");

			string appDataPath = AppSettings.GetRequiredValue<string>("AppDataPath");
			string storedPlaylistFileName = Path.Combine(appDataPath, "Playlist.json");

			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepositoryEF>(new InjectionConstructor());
			DIContainer.RegisterType<ISongTagger, SongTagger>();
			DIContainer.RegisterType<IMusicLibraryStorage, FileSystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), typeof(ISongTagger), localStorageRoot));
			DIContainer.RegisterType<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterInstance(new DiscLibrary(async () =>
			{
				var library = DIContainer.Resolve<IMusicLibrary>();
				return await library.GetDiscsAsync(true);
			}));

			DIContainer.RegisterType<IApplicationViewModelHolder, ApplicationViewModelHolder>();
			DIContainer.RegisterType<INavigatedViewModelHolder, NavigatedViewModelHolder>();
			DIContainer.RegisterType<ILibraryExplorerViewModel, LibraryExplorerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IExplorerSongListViewModel, ExplorerSongListViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ISongPlaylistViewModel, PersistentSongPlaylistViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IEditDiscPropertiesViewModel, EditDiscPropertiesViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IEditSongPropertiesViewModel, EditSongPropertiesViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IMusicPlayerViewModel, MusicPlayerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IDiscAdviserViewModel, DiscAdviserViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IRateDiscViewModel, RateDiscViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILoggerViewModel, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IDiscArtViewModel, DiscArtViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IEditDiscArtViewModel, EditDiscArtViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ApplicationViewModel>(new ContainerControlledLifetimeManager());

			DIContainer.RegisterType<ILibraryBrowser, FileSystemLibraryBrowser>();
			DIContainer.RegisterType<ITimerFacade, TimerFacade>(new InjectionConstructor());
			DIContainer.RegisterType<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			DIContainer.RegisterType<ILastFMApiClient, LastFMApiClient>(new InjectionConstructor(typeof(ITokenAuthorizer), lastFMApiKey, lastFMSharedSecret, lastFMSessionKey));
			DIContainer.RegisterType<IScrobbler, LastFMScrobbler>(new InjectionConstructor(typeof(ILastFMApiClient)));
			DIContainer.RegisterType<IMessageLogger, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILibraryContentUpdater, LibraryContentUpdater>();
			DIContainer.RegisterType<IViewNavigator, ViewNavigator>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			DIContainer.RegisterType<IDiscGroupper, MyLibraryDiscGroupper>();
			DIContainer.RegisterType<IDiscGroupSorter, RankBasedDiscSorter>();
			DIContainer.RegisterType<ILibraryStructurer, MyLibraryStructurer>();
			DIContainer.RegisterType<IWindowService, WpfWindowService>();
			DIContainer.RegisterType<IPlaylistDataRepository, JsonFilePlaylistDataRepository>(new InjectionConstructor(typeof(IFileSystemFacade), storedPlaylistFileName));
			DIContainer.RegisterType<IAudioPlayer, AudioPlayer>();
			DIContainer.RegisterType<IMediaPlayerFacade, MediaPlayerFacade>();
			DIContainer.RegisterType<ISongPlaybacksRegistrator, SongPlaybacksRegistrator>();
			DIContainer.RegisterType<IClock, SystemClock>();
			DIContainer.RegisterType<IDocumentDownloader, HttpDocumentDownloader>();
			DIContainer.RegisterType<IWebBrowser, SystemDefaultWebBrowser>();
			DIContainer.RegisterType<IDiscArtValidator, DiscArtValidator>();
		}
	}
}
