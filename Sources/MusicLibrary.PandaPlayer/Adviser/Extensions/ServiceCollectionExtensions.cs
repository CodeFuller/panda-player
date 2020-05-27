using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.PandaPlayer.Adviser.Grouping;
using MusicLibrary.PandaPlayer.Adviser.Interfaces;
using MusicLibrary.PandaPlayer.Adviser.Internal;
using MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers;
using MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser;
using MusicLibrary.PandaPlayer.Adviser.Settings;

namespace MusicLibrary.PandaPlayer.Adviser.Extensions
{
	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPlaylistAdviser(this IServiceCollection services, Action<AdviserSettings> setupSettings)
		{
			services.AddSingleton<IDiscGroupper, FolderDiscGrouper>();
			services.AddSingleton<IDiscGroupSorter, RankBasedDiscGroupSorter>();
			services.AddSingleton<IAdviseFactorsProvider, AdviseFactorsProvider>();

			services.AddSingleton<RankBasedDiscAdviser>();
			services.AddSingleton<HighlyRatedSongsAdviser>();
			services.AddSingleton<FavouriteArtistDiscsAdviser>(sp =>
				ActivatorUtilities.CreateInstance<FavouriteArtistDiscsAdviser>(sp, sp.GetRequiredService<RankBasedDiscAdviser>()));

			services.AddTransient<ICompositePlaylistAdviser, CompositePlaylistAdviser>(sp =>
				ActivatorUtilities.CreateInstance<CompositePlaylistAdviser>(sp, sp.GetRequiredService<RankBasedDiscAdviser>(),
					sp.GetRequiredService<HighlyRatedSongsAdviser>(), sp.GetRequiredService<FavouriteArtistDiscsAdviser>()));

			services.Configure(setupSettings);
			services.Configure<FavouriteArtistsAdviserSettings>(s => setupSettings(new AdviserSettings { FavouriteArtistsAdviser = s }));
			services.Configure<HighlyRatedSongsAdviserSettings>(s => setupSettings(new AdviserSettings { HighlyRatedSongsAdviser = s }));

			return services;
		}
	}
}
