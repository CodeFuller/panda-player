using System;
using CF.MusicLibrary.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CF.MusicLibrary.Dal.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDal(this IServiceCollection services, Action<SqLiteConnectionSettings> setupSettings)
		{
			var settings = new SqLiteConnectionSettings();
			setupSettings(settings);
			services.AddSingleton<Action<DbContextOptionsBuilder>>(settings.ToContextSetup());

			services.AddTransient<IMusicLibraryRepository, MusicLibraryRepositoryEF>();
			services.AddTransient<IDataCopier, DataCopier>();

			return services;
		}
	}
}
