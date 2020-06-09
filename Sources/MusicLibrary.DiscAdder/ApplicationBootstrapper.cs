using CF.Library.Bootstrap;
using CF.Library.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.DiscAdder.Internal;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ParsingContent;
using MusicLibrary.DiscAdder.ParsingSong;
using MusicLibrary.DiscAdder.ViewModels;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.Services.Extensions;
using MusicLibrary.Services.Media;
using MusicLibrary.Shared;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.DiscAdder
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddDiscTitleToAlbumMapper(settings => configuration.Bind("discToAlbumMappings", settings));

			services.Configure<DiscPreprocessorSettings>(configuration.Bind);

			services.AddLocalDbDal(settings => configuration.Bind("localDb:dataStorage", settings));
			services.AddMusicLibraryDbContext(configuration.GetConnectionString("musicLibraryDb"));
			services.AddMusicLibraryServices();
			services.AddDiscTitleToAlbumMapper(settings => configuration.Bind("discToAlbumMappings", settings));

			RegisterViewModels(services);
			services.AddImages();
			services.AddSingleton<IObjectFactory<IImageFile>, ObjectFactory<IImageFile>>();

			services.AddTransient<IWorkshopMusicStorage, WorkshopMusicStorage>();

			services.AddTransient<IEthalonSongParser, EthalonSongParser>();
			services.AddTransient<IEthalonDiscParser, EthalonDiscParser>();
			services.AddTransient<IDiscContentParser, DiscContentParser>();
			services.AddTransient<IInputContentSplitter, InputContentSplitter>();
			services.AddTransient<IDiscContentComparer, DiscContentComparer>();
			services.AddTransient<ISongMediaInfoProvider, SongMediaInfoProvider>();
			services.AddTransient<IContentCrawler, ContentCrawler>();
			services.AddTransient<ISourceFileTypeResolver, SourceFileTypeResolver>();
			services.AddSingleton<IFolderProvider, FolderProvider>();
		}

		private static void RegisterViewModels(IServiceCollection services)
		{
			services.AddTransient<IEditSourceContentViewModel, EditSourceContentViewModel>();
			services.AddTransient<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			services.AddTransient<IEditSourceDiscImagesViewModel, EditSourceDiscImagesViewModel>();
			services.AddTransient<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			services.AddTransient<IAddToLibraryViewModel, AddToLibraryViewModel>();
			services.AddTransient<ApplicationViewModel>();
		}
	}
}
