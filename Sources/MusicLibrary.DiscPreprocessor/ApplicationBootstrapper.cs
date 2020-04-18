using CF.Library.Bootstrap;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Common.Images;
using MusicLibrary.Core;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Media;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.Extensions;
using MusicLibrary.DiscPreprocessor.Interfaces;
using MusicLibrary.DiscPreprocessor.MusicStorage;
using MusicLibrary.DiscPreprocessor.ParsingContent;
using MusicLibrary.DiscPreprocessor.ParsingSong;
using MusicLibrary.DiscPreprocessor.ViewModels;
using MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using MusicLibrary.Library;
using MusicLibrary.Tagger;

namespace MusicLibrary.DiscPreprocessor
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<DiscPreprocessorSettings>(configuration.Bind);
			services.Configure<DiscToAlbumMappingSettings>(options => configuration.Bind("discToAlbumMappings", options));

			services.AddDal(settings => configuration.Bind("database", settings));

			services.AddTransient<IFileStorage, FileSystemStorage>();
			services.AddTransient<IMusicLibraryStorage, FileSystemMusicStorage>();
			services.AddTransient<IChecksumCalculator, Crc32Calculator>();
			services.AddTransient<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			services.AddTransient<IFileSystemFacade, FileSystemFacade>();
			services.AddTransient<IWorkshopMusicStorage, WorkshopMusicStorage>();

			services.AddSingleton<DiscLibrary>(sp => new DiscLibrary(async () =>
			{
				var library = sp.GetRequiredService<IMusicLibrary>();
				return await library.LoadDiscs();
			}));

			services.AddTransient<IEthalonSongParser, EthalonSongParser>();
			services.AddTransient<IEthalonDiscParser, EthalonDiscParser>();
			services.AddTransient<IDiscContentParser, DiscContentParser>();
			services.AddTransient<IInputContentSplitter, InputContentSplitter>();
			services.AddTransient<IDiscContentComparer, DiscContentComparer>();
			services.AddTransient<ISongTagger, SongTagger>();
			services.AddTransient<ISongMediaInfoProvider, SongMediaInfoProvider>();
			services.AddTransient<ILibraryStructurer, LibraryStructurer>();
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFile, ImageFile>();
			services.AddTransient<IObjectFactory<IImageFile>, ObjectFactory<IImageFile>>();
			services.AddTransient<IContentCrawler, ContentCrawler>();
			services.AddTransient<ISourceFileTypeResolver, SourceFileTypeResolver>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();
			services.AddSingleton<IDiscTitleToAlbumMapper, DiscTitleToAlbumMapper>();

			services.AddTransient<IEditSourceContentViewModel, EditSourceContentViewModel>();
			services.AddTransient<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			services.AddTransient<IEditSourceDiscImagesViewModel, EditSourceDiscImagesViewModel>();
			services.AddTransient<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			services.AddTransient<IAddToLibraryViewModel, AddToLibraryViewModel>();
			services.AddTransient<ApplicationViewModel>();
		}
	}
}
