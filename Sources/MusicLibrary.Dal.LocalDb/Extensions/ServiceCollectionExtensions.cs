using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Dal.Abstractions.Interfaces;
using MusicLibrary.Dal.Extensions;
using MusicLibrary.Logic.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLocalDbDal(this IServiceCollection services, Action<SqLiteConnectionSettings> setupSettings)
		{
			services.AddDal(setupSettings);

			services.AddSingleton<IFoldersRepository, FoldersRepository>();
			services.AddSingleton<IDiscsRepository, DiscsRepository>();
			services.AddSingleton<IGenresRepository, GenresRepository>();
			services.AddSingleton<IArtistsRepository, ArtistsRepository>();

			return services;
		}
	}
}
