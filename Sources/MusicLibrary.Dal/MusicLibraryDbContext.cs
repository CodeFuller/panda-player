﻿using System;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Dal
{
	internal class MusicLibraryDbContext : DbContext
	{
		private readonly Action<DbContextOptionsBuilder> contextSetup;

		public MusicLibraryDbContext(Action<DbContextOptionsBuilder> contextSetup)
		{
			this.contextSetup = contextSetup ?? throw new ArgumentNullException(nameof(contextSetup));
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			contextSetup(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Artist>(builder =>
			{
				builder.ToTable("Artists");

				builder.Property(a => a.Name).IsRequired();
			});

			modelBuilder.Entity<Disc>(builder =>
			{
				builder.ToTable("Discs");

				builder
					.Ignore(d => d.Artist)
					.Ignore(d => d.Genre)
					.Ignore(d => d.Year)
					.Ignore(d => d.Uri)
					.Ignore(d => d.LastPlaybackTime)
					.Ignore(d => d.PlaybacksPassed)
					.Ignore(d => d.Songs)
					.Ignore(d => d.AllSongs)
					.Ignore(d => d.RepresentativeSongs)
					.Ignore(d => d.CoverImage)
					.Ignore(d => d.IsDeleted);

				builder.Property(d => d.Title).IsRequired();
				builder.Property(d => d.DiscUri).IsRequired().HasColumnName("Uri");
			});

			modelBuilder.Entity<Genre>(builder =>
			{
				builder.ToTable("Genres");

				builder.Property(g => g.Name).IsRequired();
			});

			modelBuilder.Entity<Song>(builder =>
			{
				builder.ToTable("Songs");

				builder.Property(s => s.Title).IsRequired();
				builder.Ignore(s => s.Duration);
				builder.Property(s => s.DurationInMilliseconds).IsRequired().HasColumnName("Duration");
				builder.Ignore(s => s.Uri);
				builder.Property(s => s.SongUri).IsRequired().HasColumnName("Uri");
				builder.Property(s => s.Checksum).IsRequired();
				builder.Ignore(s => s.IsDeleted);

				builder.Property(di => di.DiscId).HasColumnName("Disc_Id");
				builder.Property(s => s.ArtistId).HasColumnName("Artist_Id");
				builder.Property(s => s.GenreId).HasColumnName("Genre_Id");
			});

			modelBuilder.Entity<Playback>(builder =>
			{
				builder.ToTable("Playbacks");

				builder.Property(s => s.SongId).HasColumnName("Song_Id");
			});

			modelBuilder.Entity<DiscImage>(builder =>
			{
				builder.ToTable("DiscImages");

				builder.Ignore(di => di.Uri);
				builder.Property(di => di.ImageUri).IsRequired().HasColumnName("Uri");
				builder.Property(di => di.DiscId).HasColumnName("Disc_Id");
			});
		}

		public DbSet<Artist> Artists { get; set; }

		public DbSet<Disc> Discs { get; set; }

		public DbSet<Song> Songs { get; set; }

		public DbSet<Genre> Genres { get; set; }
	}
}