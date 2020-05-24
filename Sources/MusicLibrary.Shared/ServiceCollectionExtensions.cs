using CF.Library.Core.Facades;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.Shared
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddImages(this IServiceCollection services)
		{
			services.AddTransient<IDiscImageValidator, DiscImageValidator>();
			services.AddTransient<IImageFacade, ImageFacade>();
			services.AddTransient<IImageFile, ImageFile>();
			services.AddTransient<IImageInfoProvider, ImageInfoProvider>();
			services.AddSingleton<IFileSystemFacade, FileSystemFacade>();

			return services;
		}
	}
}
