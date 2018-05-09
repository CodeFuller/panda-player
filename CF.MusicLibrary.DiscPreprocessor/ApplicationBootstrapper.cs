using CF.Library.Bootstrap;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ParsingContent;
using CF.MusicLibrary.DiscPreprocessor.ParsingSong;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.Tagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CF.MusicLibrary.DiscPreprocessor
{
	internal class ApplicationBootstrapper : DiApplicationBootstrapper<ApplicationViewModel>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<FileSystemStorageSettings>(options => configuration.Bind("fileSystemStorage", options));
			services.Configure<DiscPreprocessorSettings>(configuration.Bind);

			services.AddTransient<IConfiguredDbConnectionFactory, SqLiteConnectionFactory>();
			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
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
			services.AddTransient<ILibraryStructurer, MyLibraryStructurer>();
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFile, ImageFile>();
			services.AddTransient<IObjectFactory<IImageFile>, ObjectFactory<IImageFile>>();
			services.AddTransient<IContentCrawler, ContentCrawler>();
			services.AddTransient<ISourceFileTypeResolver, SourceFileTypeResolver>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();

			services.AddTransient<IEditSourceContentViewModel, EditSourceContentViewModel>();
			services.AddTransient<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			services.AddTransient<IEditSourceDiscImagesViewModel, EditSourceDiscImagesViewModel>();
			services.AddTransient<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			services.AddTransient<IAddToLibraryViewModel, AddToLibraryViewModel>();
			services.AddTransient<ApplicationViewModel>();
		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			base.BootstrapConfiguration(configurationBuilder, commandLineArgs);

			configurationBuilder.AddJsonFile("AppSettings.Dev.json", optional: true);
		}
	}
}
