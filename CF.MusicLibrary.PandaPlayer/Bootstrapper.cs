using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.PandaPlayer.Player;
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
		protected override void RegisterDependencies()
		{
			DIContainer.LoadConfiguration();
			AppSettings.SettingsProvider = DIContainer.Resolve<ISettingsProvider>();

			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");

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
		}
	}
}
