using System;
using CF.Library.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.DiscAdder.Internal;
using MusicLibrary.DiscAdder.MusicStorage;
using MusicLibrary.DiscAdder.ParsingContent;
using MusicLibrary.DiscAdder.ParsingSong;
using MusicLibrary.DiscAdder.ViewModels;
using MusicLibrary.DiscAdder.ViewModels.Interfaces;
using MusicLibrary.DiscAdder.ViewModels.SourceContent;
using MusicLibrary.Services.Media;
using MusicLibrary.Shared;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.DiscAdder.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDiscAdder(this IServiceCollection services, Action<DiscAdderSettings> setupSettings)
		{
			services.Configure<DiscAdderSettings>(setupSettings);

			RegisterViewModels(services);

			services.AddImages();
			services.AddSingleton<IObjectFactory<IImageFile>, ObjectFactory<IImageFile>>();

			services.AddSingleton<IWorkshopMusicStorage, WorkshopMusicStorage>();

			services.AddSingleton<IReferenceSongParser, ReferenceSongParser>();
			services.AddSingleton<IReferenceDiscParser, ReferenceDiscParser>();
			services.AddSingleton<IDiscContentParser, DiscContentParser>();
			services.AddSingleton<IInputContentSplitter, InputContentSplitter>();
			services.AddSingleton<IDiscContentComparer, DiscContentComparer>();
			services.AddSingleton<ISongMediaInfoProvider, SongMediaInfoProvider>();
			services.AddSingleton<IContentCrawler, ContentCrawler>();
			services.AddSingleton<ISourceFileTypeResolver, SourceFileTypeResolver>();
			services.AddSingleton<IFolderProvider, FolderProvider>();

			return services;
		}

		private static void RegisterViewModels(IServiceCollection services)
		{
			services.AddSingleton<IReferenceContentViewModel, ReferenceContentViewModel>();
			services.AddSingleton<IEditSourceContentViewModel, EditSourceContentViewModel>();
			services.AddSingleton<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			services.AddSingleton<IEditSourceDiscImagesViewModel, EditSourceDiscImagesViewModel>();
			services.AddSingleton<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			services.AddSingleton<IAddToLibraryViewModel, AddToLibraryViewModel>();
			services.AddSingleton<IDiscAdderViewModel, DiscAdderViewModel>();
		}
	}
}
