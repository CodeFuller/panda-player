using System.Data.Common;
using System.Data.Entity;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Dal
{
	public class MusicLibraryEntities : DbContext
	{
		public MusicLibraryEntities(DbConnection dbConnection)
			: base(dbConnection, true)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Artist>().Property(a => a.Name).IsRequired();

			modelBuilder.Entity<Disc>().Ignore(d => d.Artist);
			modelBuilder.Entity<Disc>().Ignore(d => d.Genre);
			modelBuilder.Entity<Disc>().Ignore(d => d.Year);
			modelBuilder.Entity<Disc>().Property(d => d.Title).IsRequired();
			modelBuilder.Entity<Disc>().Ignore(d => d.Uri);
			modelBuilder.Entity<Disc>().Property(d => d.DiscUri).IsRequired().HasColumnName("Uri");
			modelBuilder.Entity<Disc>().Ignore(d => d.LastPlaybackTime);
			modelBuilder.Entity<Disc>().Ignore(d => d.PlaybacksPassed);
			modelBuilder.Entity<Disc>().Ignore(d => d.Songs);
			modelBuilder.Entity<Disc>().Ignore(d => d.IsDeleted);
			modelBuilder.Entity<Disc>().Ignore(d => d.CoverImage);

			modelBuilder.Entity<Genre>().Property(g => g.Name).IsRequired();

			modelBuilder.Entity<Song>().Property(s => s.Title).IsRequired();
			modelBuilder.Entity<Song>().Ignore(s => s.Duration);
			modelBuilder.Entity<Song>().Property(s => s.DurationInMilliseconds).IsRequired().HasColumnName("Duration");
			modelBuilder.Entity<Song>().Ignore(s => s.Uri);
			modelBuilder.Entity<Song>().Property(s => s.SongUri).IsRequired().HasColumnName("Uri");
			modelBuilder.Entity<Song>().Property(s => s.Checksum).IsRequired();
			modelBuilder.Entity<Song>().Ignore(s => s.IsDeleted);
			modelBuilder.Entity<Song>().Property(s => s.ArtistId).HasColumnName("Artist_Id");
			modelBuilder.Entity<Song>().Property(s => s.GenreId).HasColumnName("Genre_Id");

			modelBuilder.Entity<DiscImage>().Ignore(image => image.Uri);
			modelBuilder.Entity<DiscImage>().Property(image => image.ImageUri).IsRequired().HasColumnName("Uri");
		}

		public DbSet<Artist> Artists { get; set; }

		public DbSet<Disc> Discs { get; set; }

		public DbSet<Song> Songs { get; set; }

		public DbSet<Genre> Genres { get; set; }
	}
}
