namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal abstract class BasicInputArtistItem
	{
		public abstract string ViewTitle { get; }

		public abstract string ArtistName { get; }

		public override string ToString()
		{
			return ViewTitle;
		}
	}
}
