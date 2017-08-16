using System.ComponentModel.DataAnnotations;

namespace CF.MusicLibrary.BL.Objects
{
	public class Genre
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
