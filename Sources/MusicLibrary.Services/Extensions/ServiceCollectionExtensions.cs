using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.Services.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMusicLibraryServices(this IServiceCollection services)
		{
			services.AddSingleton<IFoldersService, FoldersService>();
			services.AddSingleton<IDiscsService, DiscsService>();
			services.AddSingleton<ISongsService, SongsService>();
			services.AddSingleton<IGenresService, GenresService>();
			services.AddSingleton<IArtistsService, ArtistsService>();
			services.AddSingleton<IStatisticsService, StatisticsService>();

			return services;
		}
	}
}
