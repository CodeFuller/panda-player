﻿using System.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using CF.Library.Core.Bootstrap;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.DiscAdviser;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace CF.MusicLibrary.AlbumAdviser
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		protected override void RegisterDependencies()
		{
			DIContainer.LoadConfiguration();
			AppSettings.SettingsProvider = DIContainer.Resolve<ISettingsProvider>();

			string mediaMonkeyStorageRoot = AppSettings.GetRequiredValue<string>("MediaMonkeyStorageRoot");
			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];

			DIContainer.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			DIContainer.RegisterType<ILibraryBuilder, LibraryBuilder>();
			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>(
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString, mediaMonkeyStorageRoot));
			DIContainer.RegisterType<ILibraryBuilder, LibraryBuilder>();
			DIContainer.RegisterType<IMusicCatalog, MusicCatalog>();
			DIContainer.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, false));
			DIContainer.RegisterType<IMusicLibrary, CatalogBasedMusicLibrary>();
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterType<IStorageUrlBuilder, StorageUrlBuilder>();

			DIContainer.RegisterType<IArtistLibraryBuilder, ArtistLibraryBuilder>();
			DIContainer.RegisterType<IDiscArtistGroupper, MyLocalLibraryArtistGroupper>();
			DIContainer.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			DIContainer.RegisterType<IRankCalculator, RankCalculator>();
			DIContainer.RegisterType<IApplicationLogic, AlbumAdviserApplicationLogic>();
		}
	}
}