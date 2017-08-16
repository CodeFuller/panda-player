using System.Data.Entity;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Dal
{
	public class MusicLibraryEntities : DbContext
	{
		public MusicLibraryEntities() :
			base("name=MusicLibraryEntities")
		{
		}

		public DbSet<Artist> Artists { get; set; }

		public DbSet<Disc> Discs { get; set; }

		public DbSet<Song> Songs { get; set; }
	}
}
