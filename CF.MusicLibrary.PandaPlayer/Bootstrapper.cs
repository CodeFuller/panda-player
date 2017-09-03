using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.PandaPlayer.Player;
using CF.MusicLibrary.PandaPlayer.Scrobbler;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace CF.MusicLibrary.PandaPlayer
{
	internal class Bootstrapper : UnityBootstrapper<ApplicationViewModel>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by DI Container.")]
		protected override void RegisterDependencies()
		{
			DIContainer.LoadConfiguration();
			AppSettings.SettingsProvider = DIContainer.Resolve<ISettingsProvider>();

			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");
			string appDataDirectory = AppSettings.GetRequiredValue<string>("AppDataPath");

			DIContainer.RegisterType<ILibraryBrowser, FileSystemLibraryBrowser>();
			DIContainer.RegisterType<ILibraryExplorerViewModel, LibraryExplorerViewModel>();
			DIContainer.RegisterType<ISongListViewModel, SongListViewModel>();
			DIContainer.RegisterType<IMusicPlayerViewModel, MusicPlayerViewModel>();
			DIContainer.RegisterType<ISongPlaylist, SongPlaylist>();
			DIContainer.RegisterType<ApplicationViewModel>();
			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			DIContainer.RegisterType<IMusicCatalog, MusicCatalog>();
			DIContainer.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, false));
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterType<ITimerFacade, TimerFacade>(new InjectionConstructor());
			DIContainer.RegisterType<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			//	CF TEMP: Store session data in database
			DIContainer.RegisterType<ILastFMApiClient, LastFMApiClient>(new InjectionConstructor(typeof(ITokenAuthorizer), @"66b7aec24069590c0d674448f7e0538d", @"2ba2f3f93caedbb3816aafefdbb4ebaa", @"qDaJ5D15454f2XPHSOytLE0yDLrUqmX2"));
			DIContainer.RegisterType<IScrobbler, PersistentScrobbler>(new InjectionConstructor(typeof(ILastFMApiClient), appDataDirectory));
			DIContainer.RegisterType<IMessageLogger, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILoggerViewModel, LoggerViewModel>(new ContainerControlledLifetimeManager());
		}
	}
}
