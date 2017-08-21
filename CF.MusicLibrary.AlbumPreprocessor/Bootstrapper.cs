using System.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using CF.Library.Core.Configuration;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	internal class Bootstrapper : UnityBootstrapper<ApplicationViewModel>
	{
		[SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by DI Container.")]
		protected override void RegisterDependencies()
		{
			DIContainer.LoadConfiguration();
			AppSettings.SettingsProvider = DIContainer.Resolve<ISettingsProvider>();

			string workshopDirectory = AppSettings.GetRequiredValue<string>("WorkshopDirectory");
			string mediaMonkeyStorageRoot = AppSettings.GetRequiredValue<string>("MediaMonkeyStorageRoot");
			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];
			if (mediaMonkeyConnectionString == null)
			{
				throw new RequiredSettingIsMissingException("MediaMonkeyDB");
			}
			bool deleteSourceContentAfterAdding = AppSettings.GetRequiredValue<bool>("DeleteSourceContentAfterAdding");

			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterType<IEthalonSongParser, EthalonSongParser>();
			DIContainer.RegisterType<IEthalonAlbumParser, EthalonAlbumParser>();
			DIContainer.RegisterType<IAlbumContentParser, AlbumContentParser>();
			DIContainer.RegisterType<IInputContentSplitter, InputContentSplitter>();
			DIContainer.RegisterType<IAlbumContentComparer, AlbumContentComparer>();
			DIContainer.RegisterType<IWorkshopMusicStorage, LocalWorkshopMusicStorage>(new InjectionConstructor(workshopDirectory));
			DIContainer.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>(
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString, mediaMonkeyStorageRoot));
			DIContainer.RegisterType<ILibraryBuilder, LibraryBuilder>();
			DIContainer.RegisterType<IMusicCatalog, MusicCatalog>();
			DIContainer.RegisterType<IMusicStorage, FilesystemMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), localStorageRoot, deleteSourceContentAfterAdding));
			DIContainer.RegisterType<IMusicLibrary, CatalogBasedMusicLibrary>(new ContainerControlledLifetimeManager());
			DIContainer.RegisterType<IArtistLibraryBuilder, ArtistLibraryBuilder>();
			DIContainer.RegisterType<IDiscArtistGroupper, MyLocalLibraryArtistGroupper>();
			DIContainer.RegisterType<IStorageUrlBuilder, StorageUrlBuilder>();
			DIContainer.RegisterType<ISongTagger, SongTagger>();

			DIContainer.RegisterType<IEditSourceContentViewModel, EditSourceContentViewModel>();
			DIContainer.RegisterType<IEditAlbumsDetailsViewModel, EditAlbumsDetailsViewModel>();
			DIContainer.RegisterType<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			DIContainer.RegisterType<IAddToLibraryViewModel, AddToLibraryViewModel>();
			DIContainer.RegisterType<ApplicationViewModel>();
		}
	}
}
