using Microsoft.EntityFrameworkCore;
using MusicLibrary.Dal.LocalDb.Entities;

namespace MusicLibrary.Dal.LocalDb.Internal
{
	internal class MusicLibraryDbContext : DbContext
	{
		public MusicLibraryDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ArtistEntity>(builder =>
			{
				builder.ToTable("Artists");

				builder.Property(a => a.Name).IsRequired();
			});

			modelBuilder.Entity<DiscEntity>(builder =>
			{
				builder.ToTable("Discs");

				builder.Property(d => d.Title).IsRequired();
				builder.Property(d => d.Uri).IsRequired();
			});

			modelBuilder.Entity<GenreEntity>(builder =>
			{
				builder.ToTable("Genres");

				builder.Property(g => g.Name).IsRequired();
			});

			modelBuilder.Entity<SongEntity>(builder =>
			{
				builder.ToTable("Songs");

				builder.Property(s => s.Title).IsRequired();
				builder.Property(s => s.Uri).IsRequired();
				builder.Property(s => s.DurationInMilliseconds).HasColumnName("Duration");

				builder.Property(s => s.DiscId).HasColumnName("Disc_Id");
				builder.Property(s => s.ArtistId).HasColumnName("Artist_Id");
				builder.Property(s => s.GenreId).HasColumnName("Genre_Id");
			});

			modelBuilder.Entity<PlaybackEntity>(builder =>
			{
				builder.ToTable("Playbacks");

				builder.Property(p => p.SongId).HasColumnName("Song_Id");
			});

			modelBuilder.Entity<DiscImageEntity>(builder =>
			{
				builder.ToTable("DiscImages");

				builder.Property(di => di.Uri).IsRequired();
				builder.Property(di => di.DiscId).HasColumnName("Disc_Id");
			});
		}

		public DbSet<ArtistEntity> Artists { get; set; }

		public DbSet<DiscEntity> Discs { get; set; }

		public DbSet<SongEntity> Songs { get; set; }

		public DbSet<GenreEntity> Genres { get; set; }
	}
}
