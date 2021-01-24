using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.LastFM.Interfaces;
using MusicLibrary.LastFM.Internal;

namespace MusicLibrary.LastFM.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLastFmScrobbler(this IServiceCollection services, Action<LastFmClientSettings> setupSettings)
		{
			services.AddTransient<ILastFMApiClient, LastFMApiClient>();
			services.AddTransient<IScrobbler, LastFMScrobbler>();

			services.Configure<LastFmClientSettings>(setupSettings);

			return services;
		}

		public static IServiceCollection WrapScrobbler<TScrobbler>(this IServiceCollection services, ServiceLifetime lifetime)
			where TScrobbler : class, IScrobbler
		{
			services.AddTransient<LastFMScrobbler>();

			static TScrobbler Factory(IServiceProvider sp) => ActivatorUtilities.CreateInstance<TScrobbler>(sp, sp.GetRequiredService<LastFMScrobbler>());
			services.Add(new ServiceDescriptor(typeof(IScrobbler), Factory, lifetime));

			return services;
		}
	}
}
