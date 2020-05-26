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

			modelBuilder.Entity<FolderEntity>(builder =>
			{
				builder.ToTable("Folders");

				builder.Property(f => f.Name).IsRequired();
				builder.Property(s => s.ParentFolderId).HasColumnName("ParentFolder_Id");

				builder.HasOne(f => f.ParentFolder)
					.WithMany(f => f.Subfolders)
					.HasForeignKey(d => d.ParentFolderId)
					.OnDelete(DeleteBehavior.Restrict);

				var rootFolder = new FolderEntity
				{
					Id = 1,
					Name = "<ROOT>",
					ParentFolderId = null,
				};

				// TODO: Check whether database seeding work and whether next id is correctly assigned to 2.
				builder.HasData(rootFolder);
			});

			modelBuilder.Entity<DiscEntity>(builder =>
			{
				builder.ToTable("Discs");

				builder.Property(d => d.Title).IsRequired();
				builder.Property(d => d.TreeTitle).IsRequired();
				builder.Property(d => d.FolderId).HasColumnName("Folder_Id");
			});

			modelBuilder.Entity<ArtistEntity>(builder =>
			{
				builder.ToTable("Artists");

				builder.Property(a => a.Name).IsRequired();
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
				builder.Property(s => s.TreeTitle).IsRequired();
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

				builder.Property(di => di.DiscId).HasColumnName("Disc_Id");
			});
		}

		public DbSet<FolderEntity> Folders { get; set; }

		public DbSet<DiscEntity> Discs { get; set; }

		public DbSet<ArtistEntity> Artists { get; set; }

		public DbSet<GenreEntity> Genres { get; set; }

		public DbSet<SongEntity> Songs { get; set; }
	}
}
