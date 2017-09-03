using CF.Library.Core.Bootstrap;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Logging;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.LastFM;
using CF.MusicLibrary.LibraryChecker.Checkers;
using CF.MusicLibrary.LibraryChecker.Registrators;
using CF.MusicLibrary.Tagger;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace CF.MusicLibrary.LibraryChecker
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object lifetime equals to the host process lifetime")]
		protected override void RegisterDependencies()
		{
			DIContainer.LoadConfiguration();
			AppSettings.SettingsProvider = DIContainer.Resolve<ISettingsProvider>();

			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");

			DIContainer.RegisterType<IApplicationLogic, ApplicationLogic>();
			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			DIContainer.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, false));
			DIContainer.RegisterType<ISongTagger, SongTagger>();
			DIContainer.RegisterType<ITokenAuthorizer, DefaultBrowserTokenAuthorizer>();
			DIContainer.RegisterType<ILastFMApiClient, LastFMApiClient>(new InjectionConstructor(typeof(ITokenAuthorizer), @"66b7aec24069590c0d674448f7e0538d", @"2ba2f3f93caedbb3816aafefdbb4ebaa", @"qDaJ5D15454f2XPHSOytLE0yDLrUqmX2"));
			DIContainer.RegisterType<IMessageLogger, ConsoleLogger>(new ContainerControlledLifetimeManager(), new InjectionConstructor(true));
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();

			DIContainer.RegisterType<IDiscConsistencyChecker, DiscConsistencyChecker>();
			DIContainer.RegisterType<ITagDataConsistencyChecker, TagDataConsistencyChecker>();
			DIContainer.RegisterType<ILastFMConsistencyChecker, LastFMConsistencyChecker>();

			DIContainer.RegisterType<ILibraryInconsistencyFilter, LibraryInconsistencyFilter>();
			DIContainer.RegisterType<ILibraryInconsistencyRegistrator, InconsistencyRegistratorToLog>("Inner");
			DIContainer.RegisterType<ILibraryInconsistencyRegistrator, InconsistencyRegistratorWithFilter>(
				new InjectionConstructor(
					new ResolvedParameter<ILibraryInconsistencyRegistrator>("Inner"),
					new ResolvedParameter<ILibraryInconsistencyFilter>()));
		}
	}
}
