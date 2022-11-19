namespace PandaPlayer.Core.Models
{
	public class ArtistModel : BasicModel
	{
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
