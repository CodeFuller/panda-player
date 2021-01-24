using CF.Library.Core.Facades;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
			services.TryAddSingleton<IFileSystemFacade, FileSystemFacade>();

			return services;
		}
	}
}
