using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Core.Interfaces;

namespace MusicLibrary.Dal.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDal(this IServiceCollection services, Action<SqLiteConnectionSettings> setupSettings)
		{
			var settings = new SqLiteConnectionSettings();
			setupSettings(settings);
			services.AddSingleton<Action<DbContextOptionsBuilder>>(settings.ToContextSetup());

			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepository>();
			services.AddTransient<IDataCopier, DataCopier>();

			return services;
		}
	}
}
