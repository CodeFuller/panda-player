using System;
using CF.Library.Core.Facades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Dal.LocalDb.Repositories;
using MusicLibrary.Services.Extensions;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLocalDbDal(this IServiceCollection services, Action<FileSystemStorageSettings> setupSettings)
		{
			services.AddSingleton<IFoldersRepository, FoldersRepository>();
			services.AddSingleton<IDiscsRepository, DiscsRepository>();
			services.AddSingleton<ISongsRepository, SongsRepository>();
			services.AddSingleton<IGenresRepository, GenresRepository>();
			services.AddSingleton<IArtistsRepository, ArtistsRepository>();
			services.AddSingleton<IStorageRepository, StorageRepository>();

			services.AddSongTagger();
			services.AddSingleton<IChecksumCalculator, Crc32Calculator>();
			services.TryAddSingleton<IFileSystemFacade, FileSystemFacade>();
			services.AddSingleton<IFileStorage, FileSystemStorage>();
			services.AddSingleton<IUriTranslator, FileSystemStorage>();

			services.Configure(setupSettings);

			services.AddSingleton<IMusicLibraryDbContextFactory, MusicLibraryDbContextFactory>();

			return services;
		}

		public static IServiceCollection AddMusicLibraryDbContext(this IServiceCollection services, string connectionString)
		{
			if (String.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("The connection string for Music Library DB is not set");
			}

			services.AddDbContext<MusicLibraryDbContext>(options => options.UseSqlite(connectionString));

			return services;
		}
	}
}
