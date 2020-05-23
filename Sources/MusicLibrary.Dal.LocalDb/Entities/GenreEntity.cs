namespace MusicLibrary.Dal.LocalDb.Entities
{
	internal class GenreEntity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
