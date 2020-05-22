using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.Dal.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Logic.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ServiceCollectionExtensions
	{
		// TODO: Avoid multiple setup actions
		public static IServiceCollection AddLocalDbDal(this IServiceCollection services, Action<SqLiteConnectionSettings> setupSettings, Action<FileSystemDataStorageSettings> setupDataStorageSettings)
		{
			services.AddDal(setupSettings);

			services.AddSingleton<IFoldersRepository, FoldersRepository>();
			services.AddSingleton<IDiscsRepository, DiscsRepository>();
			services.AddSingleton<ISongsRepository, SongsRepository>();
			services.AddSingleton<IGenresRepository, GenresRepository>();
			services.AddSingleton<IArtistsRepository, ArtistsRepository>();
			services.AddSingleton<IStatisticsRepository, StatisticsRepository>();

			services.Configure<FileSystemDataStorageSettings>(setupDataStorageSettings);
			services.AddSingleton<IDataStorage, FileSystemDataStorage>();

			return services;
		}
	}
}
