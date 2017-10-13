﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.Dal
{
	public class MusicLibraryRepositoryEF : IMusicLibraryRepository
	{
		private readonly Func<MusicLibraryEntities> contextFactory;

		public MusicLibraryRepositoryEF()
		{
			contextFactory = () => new MusicLibraryEntities();
		}

		public MusicLibraryRepositoryEF(DbConnection dbConnection)
		{
			contextFactory = () => new MusicLibraryEntities(dbConnection);
		}

		private MusicLibraryEntities GetContext()
		{
			return contextFactory();
		}

		public async Task AddSong(Song song)
		{
			if (song.Playbacks.Any())
			{
				throw new InvalidOperationException("Added song could not contain any playbacks");
			}

			//	Adding will not work correctly if songs collection is filled for the Disc.
			var disc = song.Disc;
			ICollection<Song> discSongs = disc.SongsUnordered;
			disc.SongsUnordered = new Collection<Song>();
			try
			{
				var artist = song.Artist;
				var genre = song.Genre;

				using (var ctx = GetContext())
				{
					//	Is it a new Genre?
					if (genre != null)
					{
						ctx.Entry(genre).State = genre.Id == 0 ? EntityState.Added : EntityState.Unchanged;
					}

					//	Is it a new Disc?
					ctx.Entry(disc).State = disc.Id == 0 ? EntityState.Added : EntityState.Unchanged;

					//	Is it a new Artist?
					if (artist != null)
					{
						ctx.Entry(artist).State = artist.Id == 0 ? EntityState.Added : EntityState.Unchanged;
					}

					ctx.Entry(song).State = EntityState.Added;

					await ctx.SaveChangesAsync();
				}
			}
			finally
			{
				disc.SongsUnordered = discSongs;
			}
		}

		public async Task UpdateSong(Song song)
		{
			using (var ctx = GetContext())
			{
				ctx.Entry(song).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}
		}

		public async Task UpdateDisc(Disc disc)
		{
			using (var ctx = GetContext())
			{
				ctx.Entry(disc).State = EntityState.Modified;
				await ctx.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Disc>> GetDiscs()
		{
			using (var ctx = GetContext())
			{
				return await ctx.Discs
					.Include(d => d.SongsUnordered.Select(s => s.Artist))
					.Include(d => d.SongsUnordered.Select(s => s.Genre))
					.ToArrayAsync();
			}
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			using (var ctx = GetContext())
			{
				ctx.Entry(song).State = EntityState.Modified;
				ctx.Entry(song.Playbacks.Last()).State = EntityState.Added;
				await ctx.SaveChangesAsync();
			};
		}

		public static async Task CopyData(DbConnection sourceConnection, DbConnection targetConnection)
		{
			using (var sourceCtx = new MusicLibraryEntities(sourceConnection))
			using (var targetCtx = new MusicLibraryEntities(targetConnection))
			{
				var sourceDiscs = await sourceCtx.Discs
					.Include(d => d.SongsUnordered.Select(s => s.Artist))
					.Include(d => d.SongsUnordered.Select(s => s.Genre))
					.Include(d => d.SongsUnordered.Select(s => s.Playbacks))
					.ToArrayAsync();

				targetCtx.Discs.AddRange(sourceDiscs);
				await targetCtx.SaveChangesAsync();
			}
		}
	}
}
