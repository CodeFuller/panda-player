using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CF.MusicLibrary.BL.Objects
{
	public class Artist
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public ICollection<Song> Songs { get; set; } = new HashSet<Song>();
	}
}
