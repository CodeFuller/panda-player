using System.IO;
using CF.Library.Core.Configuration;
using CF.Library.Core.Exceptions;
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
using static CF.Library.Core.Extensions.FormattableStringExtensions;

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
			string lastFMApiKey = AppSettings.GetRequiredValue<string>("LastFMApiKey");

			string lastFMSharedSecret;
			string lastFMSessionKey;
			LoadLastFMSessionInfo(appDataDirectory, out lastFMSharedSecret, out lastFMSessionKey);

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
			DIContainer.RegisterType<ILastFMApiClient, LastFMApiClient>(new InjectionConstructor(typeof(ITokenAuthorizer), lastFMApiKey, lastFMSharedSecret, lastFMSessionKey));
			DIContainer.RegisterType<IScrobbler, PersistentScrobbler>(new InjectionConstructor(typeof(ILastFMApiClient), appDataDirectory));
			DIContainer.RegisterType<IMessageLogger, LoggerViewModel>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<ILoggerViewModel, LoggerViewModel>(new ContainerControlledLifetimeManager());
		}

		private static void LoadLastFMSessionInfo(string appDataDirectory, out string lastFMSharedSecret, out string lastFMSessionKey)
		{
			var sessionInfoFileName = Path.Combine(appDataDirectory, "LastFMSessionInfo.txt");
			var lines = File.ReadAllLines(sessionInfoFileName);
			if (lines.Length != 2)
			{
				throw new InvalidInputDataException(Current($"Exactly two lines expected in Last.fm session info file '{sessionInfoFileName}'"));
			}

			lastFMSharedSecret = lines[0];
			lastFMSessionKey = lines[1];
		}
	}
}
