﻿using System;
using Microsoft.Extensions.DependencyInjection;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Interfaces;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Adviser.PlaylistAdvisers;
using PandaPlayer.Adviser.RankBasedAdviser;
using PandaPlayer.Adviser.Settings;

namespace PandaPlayer.Adviser.Extensions
{
	internal static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPlaylistAdviser(this IServiceCollection services, Action<AdviserSettings> setupSettings)
		{
			services.AddSingleton<IDiscGrouper, DiscGrouper>();
			services.AddSingleton<IDiscGroupSorter, RankBasedDiscGroupSorter>();
			services.AddSingleton<IAdviseRankCalculator, AdviseRankCalculator>();

			services.AddSingleton<RankBasedDiscAdviser>();
			services.AddSingleton<HighlyRatedSongsAdviser>();
			services.AddSingleton<FavoriteArtistDiscsAdviser>(sp =>
				ActivatorUtilities.CreateInstance<FavoriteArtistDiscsAdviser>(sp, sp.GetRequiredService<RankBasedDiscAdviser>()));

			services.AddTransient<ICompositePlaylistAdviser, CompositePlaylistAdviser>(sp =>
				ActivatorUtilities.CreateInstance<CompositePlaylistAdviser>(sp, sp.GetRequiredService<RankBasedDiscAdviser>(),
					sp.GetRequiredService<HighlyRatedSongsAdviser>(), sp.GetRequiredService<FavoriteArtistDiscsAdviser>()));

			services.Configure(setupSettings);
			services.Configure<FavoriteArtistsAdviserSettings>(s => setupSettings(new AdviserSettings { FavoriteArtistsAdviser = s }));
			services.Configure<HighlyRatedSongsAdviserSettings>(s => setupSettings(new AdviserSettings { HighlyRatedSongsAdviser = s }));

			return services;
		}
	}
}
