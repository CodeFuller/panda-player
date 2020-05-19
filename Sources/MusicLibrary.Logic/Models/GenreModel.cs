namespace MusicLibrary.Logic.Models
{
	public class GenreModel
	{
		public ItemId Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
