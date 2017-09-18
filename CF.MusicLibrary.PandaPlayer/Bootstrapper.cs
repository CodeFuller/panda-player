using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.Library.Unity;
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
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using CF.MusicLibrary.Tagger;
using CF.MusicLibrary.Universal.Interfaces;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.PandaPlayer
{
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

			DIContainer.RegisterType<ILibraryBrowser, FileSystemLibraryBrowser>();
			DIContainer.RegisterType<ILibraryExplorerViewModel, LibraryExplorerViewModel>();
			DIContainer.RegisterType<IExplorerSongListViewModel, ExplorerSongListViewModel>();
			DIContainer.RegisterType<ISongPlaylistViewModel, SongPlaylistViewModel>();
			DIContainer.RegisterType<IEditSongPropertiesViewModel, EditSongPropertiesViewModel>();
			DIContainer.RegisterType<IMusicPlayerViewModel, MusicPlayerViewModel>();
			DIContainer.RegisterType<IDiscAdviserViewModel, DiscAdviserViewModel>();
			DIContainer.RegisterType<ApplicationViewModel>();
			DIContainer.RegisterType<ITimerFacade, TimerFacade>(new InjectionConstructor());
			DIContainer.RegisterType<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			DIContainer.RegisterType<ILastFMApiClient, LastFMApiClient>(new InjectionConstructor(typeof(ITokenAuthorizer), lastFMApiKey, lastFMSharedSecret, lastFMSessionKey));
			DIContainer.RegisterType<IScrobbler, LastFMScrobbler>(new InjectionConstructor(typeof(ILastFMApiClient)));
			DIContainer.RegisterType<IMessageLogger, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILoggerViewModel, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILibraryContentUpdater, LibraryContentUpdater>();
			DIContainer.RegisterType<IWindowService, WindowService>();
			DIContainer.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			DIContainer.RegisterType<IDiscGroupper, MyLibraryDiscGroupper>();
			DIContainer.RegisterType<IDiscGroupSorter, RankBasedDiscSorter>();
			DIContainer.RegisterType<ILibraryStructurer, MyLibraryStructurer>();
		}
	}
}
