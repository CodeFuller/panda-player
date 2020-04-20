using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Dal.Abstractions.Interfaces;
using MusicLibrary.Dal.Extensions;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLocalDbDal(this IServiceCollection services, Action<SqLiteConnectionSettings> setupSettings)
		{
			services.AddDal(setupSettings);

			services.AddSingleton<IFoldersRepository, FoldersRepository>();
			services.AddSingleton<IDiscsRepository, DiscsRepository>();

			return services;
		}
	}
}
