using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using CF.Library.Core.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.DiscAdviser;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.AlbumAdviser
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		protected override void RegisterDependencies(IUnityContainer container)
		{
			string mediaMonkeyStorageRoot = ReadAppSetting("MediaMonkeyStorageRoot");
			string localStorageRoot = ReadAppSetting("LocalStorageRoot");
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];

			container.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			container.RegisterType<ILibraryBuilder, LibraryBuilder>();
			container.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>(
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString, mediaMonkeyStorageRoot));
			container.RegisterType<ILibraryBuilder, LibraryBuilder>();
			container.RegisterType<IMusicCatalog, MusicCatalog>();
			container.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, false));
			container.RegisterType<IMusicLibrary, CatalogBasedMusicLibrary>();
			container.RegisterType<IFileSystemFacade, FileSystemFacade>();
			container.RegisterType<IStorageUrlBuilder, StorageUrlBuilder>();

			container.RegisterType<IArtistLibraryBuilder, ArtistLibraryBuilder>();
			container.RegisterType<IDiscArtistGroupper, MyLocalLibraryArtistGroupper>();
			container.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			container.RegisterType<IRankCalculator, RankCalculator>();
			container.RegisterType<IApplicationLogic, AlbumAdviserApplicationLogic>();
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
	}
}
