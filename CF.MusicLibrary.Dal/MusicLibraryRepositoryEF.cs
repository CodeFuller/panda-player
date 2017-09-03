using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Dal
{
	public class MusicLibraryRepositoryEF : IMusicLibraryRepository
	{
		public async Task<DiscLibrary> GetDiscLibraryAsync()
		{
			return new DiscLibrary(await GetDiscsAsync());
		}

		public async Task<IEnumerable<Artist>> GetArtistsAsync()
		{
			using (var ctx = new MusicLibraryEntities())
			{
				var artists = new List<Artist>();
				foreach (IGrouping<Artist, Song> g in await ctx.Songs
					.Where(s => s.Artist != null)
					.GroupBy(s => s.Artist).ToArrayAsync())
				{
					var artist = g.Key;
					artist.Songs = new HashSet<Song>(g);
					artists.Add(artist);
				}

				return artists;
			};
		}

		public async Task<IEnumerable<Disc>> GetDiscsAsync()
		{
			using (var ctx = new MusicLibraryEntities())
			{
				return await ctx.Discs
					.Include(d => d.SongsUnordered.Select(s => s.Artist))
					.Include(d => d.SongsUnordered.Select(s => s.Genre))
					.ToArrayAsync();
			}
		}

		public async Task<IEnumerable<Song>> GetSongsAsync()
		{
			using (var ctx = new MusicLibraryEntities())
			{
				return await ctx.Songs
					.Include(s => s.Artist)
					.Include(s => s.Disc)
					.Include(s => s.Genre)
					.ToArrayAsync();
			};
		}

		public Task<IEnumerable<Genre>> GetGenresAsync()
		{
			throw new NotImplementedException();
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			using (var ctx = new MusicLibraryEntities())
			{
				ctx.Entry(song).State = EntityState.Modified;
				ctx.Entry(song.Playbacks.Last()).State = EntityState.Added;
				await ctx.SaveChangesAsync();
			};
		}
	}
}
