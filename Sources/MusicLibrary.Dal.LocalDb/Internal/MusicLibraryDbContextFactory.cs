using System;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class MusicLibraryDbContextFactory : IMusicLibraryDbContextFactory
	{
		private readonly DbContextOptions<MusicLibraryDbContext> options;

		public MusicLibraryDbContextFactory(DbContextOptions<MusicLibraryDbContext> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public MusicLibraryDbContext Create()
		{
			return new MusicLibraryDbContext(options);
		}
	}
}
