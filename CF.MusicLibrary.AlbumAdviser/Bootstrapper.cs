using System.Configuration;
using System.Data.Common;
using CF.Library.Core.Bootstrap;
using CF.Library.Unity;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.DiscAdviser;
using CF.MusicLibrary.BL.MyLocalLibrary;
using CF.MusicLibrary.Dal.MediaMonkey;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.AlbumAdviser
{
	internal class Bootstrapper : UnityBootstrapper<IApplicationLogic>
	{
		protected override void RegisterDependencies(IUnityContainer container)
		{
			var mediaMonkeyConnectionString = ConfigurationManager.ConnectionStrings["MediaMonkeyDB"];
			var libraryRootDirectory = ConfigurationManager.AppSettings["LibraryRootDirectory"];

			container.RegisterType<DbProviderFactory>(new InjectionFactory(context =>
				DbProviderFactories.GetFactory(mediaMonkeyConnectionString.ProviderName)));
			container.RegisterType<ILibraryBuilder, LibraryBuilder>();
			container.RegisterType<IMusicLibraryRepository, MusicLibraryRepository>(
				new InjectionConstructor(typeof(DbProviderFactory), typeof(ILibraryBuilder), mediaMonkeyConnectionString.ConnectionString));

			container.RegisterType<IArtistLibraryBuilder, ArtistLibraryBuilder>();
			container.RegisterType<IDiscArtistGroupper, MyLocalLibraryArtistGroupper>(new InjectionConstructor(libraryRootDirectory));
			container.RegisterType<IDiscAdviser, RankBasedDiscAdviser>();
			container.RegisterType<IRankCalculator, RankCalculator>();
			container.RegisterType<IApplicationLogic, AlbumAdviserApplicationLogic>();
		}
	}
}
