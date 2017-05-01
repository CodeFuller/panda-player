namespace CF.MusicLibrary.BL.Objects
{
	public class Genre
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
