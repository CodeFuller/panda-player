using CF.Library.Core.Bootstrap;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.Universal.Interfaces;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.DiscAdviser
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		protected override void RegisterDependencies()
		{
			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");

			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepositoryEF>(new InjectionConstructor());
			DIContainer.RegisterType<IMusicLibraryStorage, FileSystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot));
			DIContainer.RegisterType<IMusicLibraryReader, RepositoryAndStorageMusicLibrary>();
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();

			DIContainer.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			DIContainer.RegisterType<IDiscGroupper, MyLibraryDiscGroupper>();
			DIContainer.RegisterType<IDiscGroupSorter, RankBasedDiscSorter>();
			DIContainer.RegisterType<IApplicationLogic, ApplicationLogic>();
		}
	}
}
