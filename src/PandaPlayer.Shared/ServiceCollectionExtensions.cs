using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PandaPlayer.Core.Facades;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.Shared
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
