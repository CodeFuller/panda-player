namespace PandaPlayer.Core.Models
{
	public class GenreModel : BasicModel
	{
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
