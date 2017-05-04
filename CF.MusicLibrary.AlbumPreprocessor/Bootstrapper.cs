using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Unity;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	internal class Bootstrapper : UnityBootstrapper<MainWindowModel>
	{
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by container.")]
		protected override void RegisterDependencies(IUnityContainer container)
		{
			string workshopDirectory = ReadAppSetting("WorkshopDirectory");
			string mediaMonkeyStorageRoot = ReadAppSetting("MediaMonkeyStorageRoot");
			string localStorageRoot = ReadAppSetting("LocalStorageRoot");
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];
			if (mediaMonkeyConnectionString == null)
			{
				throw new RequiredSettingIsMissingException("MediaMonkeyDB");
			}
			bool moveSongFilesToLibrary = ReadBoolAppSetting("MoveSongFilesToLibrary");

			container.RegisterType<IFileSystemFacade, FileSystemFacade>();
			container.RegisterType<IEthalonSongParser, EthalonSongParser>();
			container.RegisterType<IEthalonAlbumParser, EthalonAlbumParser>();
			container.RegisterType<IAlbumContentParser, AlbumContentParser>();
			container.RegisterType<IInputContentSplitter, InputContentSplitter>();
			container.RegisterType<IAlbumContentComparer, AlbumContentComparer>();
			container.RegisterType<IWorkshopMusicStorage, LocalWorkshopMusicStorage>(new InjectionConstructor(workshopDirectory));
			container.RegisterType<IWindowService, WpfWindowService>();
			container.RegisterType<IObjectFactory<AddToLibraryViewModel>, UnityBasedObjectFactory<AddToLibraryViewModel>>(new InjectionConstructor(container));
			container.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			container.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>(
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString, mediaMonkeyStorageRoot));
			container.RegisterType<ILibraryBuilder, LibraryBuilder>();
			container.RegisterType<IMusicCatalog, MusicCatalog>();
			container.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, moveSongFilesToLibrary));
			container.RegisterType<IMusicLibrary, CatalogBasedMusicLibrary>(new ContainerControlledLifetimeManager());
			container.RegisterType<IArtistLibraryBuilder, ArtistLibraryBuilder>();
			container.RegisterType<IDiscArtistGroupper, MyLocalLibraryArtistGroupper>();
			container.RegisterType<IStorageUrlBuilder, StorageUrlBuilder>();
			container.RegisterType<ISongTagger, SongTagger>();
			container.RegisterType<MainWindowModel>();
		}

		private static string ReadAppSetting(string settingKeyName)
		{
			string settingValue = ConfigurationManager.AppSettings[settingKeyName];
			if (String.IsNullOrEmpty(settingValue))
			{
				throw new RequiredSettingIsMissingException(settingKeyName);
			}

			return settingValue;
		}

		private static bool ReadBoolAppSetting(string settingKeyName)
		{
			string stringValue = ReadAppSetting(settingKeyName);
			bool boolValue;
			if (!Boolean.TryParse(stringValue, out boolValue))
			{
				throw new InvalidConfigurationException(Current($"Can't parse boolean value of '{boolValue}' for {settingKeyName}"));
			}

			return boolValue;
		}
	}
}
