using System.Collections.Generic;

namespace CF.MusicLibrary.BL.Objects
{
	public class Artist
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
	}
}
