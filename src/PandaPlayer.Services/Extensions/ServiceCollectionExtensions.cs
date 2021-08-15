using System;
using Microsoft.Extensions.DependencyInjection;
using PandaPlayer.Core.Facades;
using PandaPlayer.Services.Diagnostic.Checkers;
using PandaPlayer.Services.Diagnostic.Interfaces;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Internal;
using PandaPlayer.Services.Settings;
using PandaPlayer.Services.Tagging;

namespace PandaPlayer.Services.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPandaPlayerServices(this IServiceCollection services)
		{
			services.AddSingleton<IFoldersService, FoldersService>();
			services.AddSingleton<IDiscsService, DiscsService>();
			services.AddSingleton<ISongsService, SongsService>();
			services.AddSingleton<IGenresService, GenresService>();
			services.AddSingleton<IArtistsService, ArtistsService>();
			services.AddSingleton<IAdviseGroupService, AdviseGroupService>();
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
