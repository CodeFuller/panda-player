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
			using (var ctx = new MusicLibraryEntities())
			{
				var discs = await ctx.Discs
					.Include("Songs.Artist")
					.ToArrayAsync();
				return new DiscLibrary(discs);
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
