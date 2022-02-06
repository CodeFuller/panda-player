using System;
using Microsoft.Extensions.DependencyInjection;
using PandaPlayer.DiscAdder.Interfaces;
using PandaPlayer.DiscAdder.Internal;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ParsingContent;
using PandaPlayer.DiscAdder.ParsingSong;
using PandaPlayer.DiscAdder.ViewModels;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;
using PandaPlayer.Services.Media;
using PandaPlayer.Shared;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.DiscAdder.Extensions
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
			services.AddSingleton<ISourceContentChecker, SourceContentChecker>();
			services.AddSingleton<ISongMediaInfoProvider, SongMediaInfoProvider>();
			services.AddSingleton<IContentCrawler, ContentCrawler>();
			services.AddSingleton<ISourceFileTypeResolver, SourceFileTypeResolver>();
			services.AddSingleton<IFolderProvider, FolderProvider>();

			return services;
		}

		private static void RegisterViewModels(IServiceCollection services)
		{
			services.AddSingleton<IRawReferenceContentViewModel, RawReferenceContentViewModel>();
			services.AddSingleton<IReferenceContentViewModel, ReferenceContentViewModel>();
			services.AddSingleton<IActualContentViewModel, ActualContentViewModel>();
			services.AddSingleton<IEditSourceContentViewModel, EditSourceContentViewModel>();
			services.AddSingleton<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			services.AddSingleton<IEditSourceDiscImagesViewModel, EditSourceDiscImagesViewModel>();
			services.AddSingleton<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			services.AddSingleton<IAddToLibraryViewModel, AddToLibraryViewModel>();
			services.AddSingleton<IDiscAdderViewModel, DiscAdderViewModel>();
		}
	}
}
