namespace CF.MusicLibrary.Core.Objects
{
	public class Artist
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public bool IsFavourite { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
