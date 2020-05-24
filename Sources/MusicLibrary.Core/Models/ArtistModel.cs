namespace MusicLibrary.Core.Models
{
	public class ArtistModel
	{
		public ItemId Id { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
