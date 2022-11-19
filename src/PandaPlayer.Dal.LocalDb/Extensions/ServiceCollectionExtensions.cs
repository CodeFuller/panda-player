using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PandaPlayer.Core.Facades;
using PandaPlayer.Dal.LocalDb.Interfaces;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Dal.LocalDb.Repositories;
using PandaPlayer.Services.Extensions;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddLocalDbDal(this IServiceCollection services, Action<FileSystemStorageSettings> setupSettings)
		{
			services.AddSingleton<IFoldersRepository, FoldersRepository>();
			services.AddSingleton<IDiscsRepository, DiscsRepository>();
			services.AddSingleton<ISongsRepository, SongsRepository>();
			services.AddSingleton<IArtistsRepository, ArtistsRepository>();
			services.AddSingleton<IAdviseGroupRepository, AdviseGroupRepository>();
			services.AddSingleton<IAdviseSetRepository, AdviseSetRepository>();
			services.AddSingleton<ISessionDataRepository, SessionDataRepository>();
			services.AddSingleton<IDiscLibraryRepository, DiscLibraryRepository>();

			services.AddSingleton<StorageRepository>();
			services.AddSingleton<IStorageRepository>(sp => sp.GetRequiredService<StorageRepository>());
			services.AddSingleton<IContentUriProvider>(sp => sp.GetRequiredService<StorageRepository>());
			services.AddSingleton<IFileStorageOrganizer, FileStorageOrganizer>();

			services.AddSongTagger();
			services.AddSingleton<IChecksumCalculator, Crc32Calculator>();
			services.TryAddSingleton<IFileSystemFacade, FileSystemFacade>();
			services.AddSingleton<IFileStorage, FileSystemStorage>();

			services.Configure(setupSettings);

			services.AddSingleton<IDbContextFactory<MusicDbContext>, MusicDbContextFactory>();

			return services;
		}

		public static IServiceCollection AddMusicDbContext(this IServiceCollection services, string connectionString)
		{
			if (String.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("The connection string for Panda Player DB is not set");
			}

			// QuerySplittingBehavior: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
			services.AddDbContext<MusicDbContext>(
				options => options
					.UseSqlite(connectionString, sqLiteOptions => sqLiteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

			return services;
		}
	}
}
