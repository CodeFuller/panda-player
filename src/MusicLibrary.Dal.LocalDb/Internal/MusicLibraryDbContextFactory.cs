using System;
using Microsoft.EntityFrameworkCore;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class MusicLibraryDbContextFactory : IDbContextFactory<MusicLibraryDbContext>
	{
		private readonly DbContextOptions<MusicLibraryDbContext> options;

		public MusicLibraryDbContextFactory(DbContextOptions<MusicLibraryDbContext> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public MusicLibraryDbContext CreateDbContext()
		{
			return new MusicLibraryDbContext(options);
		}
	}
}
