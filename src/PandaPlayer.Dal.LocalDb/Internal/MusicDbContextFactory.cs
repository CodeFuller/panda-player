using System;
using Microsoft.EntityFrameworkCore;

namespace PandaPlayer.Dal.LocalDb.Internal
{
	internal class MusicDbContextFactory : IDbContextFactory<MusicDbContext>
	{
		private readonly DbContextOptions<MusicDbContext> options;

		public MusicDbContextFactory(DbContextOptions<MusicDbContext> options)
		{
			this.options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public MusicDbContext CreateDbContext()
		{
			return new(options);
		}
	}
}
