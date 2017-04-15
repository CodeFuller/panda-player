using System.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Unity;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ParsingContent;
using CF.MusicLibrary.AlbumPreprocessor.ParsingSong;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	internal class Bootstrapper : UnityBootstrapper<MainWindowModel>
	{
		protected override void RegisterDependencies(IUnityContainer container)
		{
			string appDataPath = ConfigurationManager.AppSettings["AppDataPath"];

			container.RegisterType<IFileSystemFacade, FileSystemFacade>();
			container.RegisterType<IEthalonSongParser, EthalonSongParser>();
			container.RegisterType<IEthalonAlbumParser, EthalonAlbumParser>();
			container.RegisterType<IAlbumContentParser, AlbumContentParser>();
			container.RegisterType<IInputContentSplitter, InputContentSplitter>();
			container.RegisterType<IAlbumContentComparer, AlbumContentComparer>();
			container.RegisterType<MainWindowModel>(new InjectionConstructor(
				new ResolvedParameter<IFileSystemFacade>(),
				new ResolvedParameter<IAlbumContentParser>(),
				new ResolvedParameter<IAlbumContentComparer>(),
				appDataPath));
		}
	}
}
