using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Core.Facades;
using MusicLibrary.Services.Diagnostic.Checkers;
using MusicLibrary.Services.Diagnostic.Interfaces;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Internal;
using MusicLibrary.Services.Settings;
using MusicLibrary.Services.Tagging;

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
			services.AddSingleton<ISessionDataService, SessionDataService>();
			services.AddSingleton<IStatisticsService, StatisticsService>();
			services.AddSingleton<IDiagnosticService, DiagnosticService>();

			services.AddSingleton<IDiscConsistencyChecker, DiscConsistencyChecker>();

			services.AddSingleton<IClock, SystemClock>();

			return services;
		}

		public static IServiceCollection AddDiscTitleToAlbumMapper(this IServiceCollection services, Action<DiscToAlbumMappingSettings> setupSettings)
		{
			services.AddSingleton<IDiscTitleToAlbumMapper, DiscTitleToAlbumMapper>();

			services.Configure<DiscToAlbumMappingSettings>(setupSettings);

			return services;
		}

		public static IServiceCollection AddSongTagger(this IServiceCollection services)
		{
			services.AddSingleton<ISongTagger, SongTagger>();

			return services;
		}
	}
}
