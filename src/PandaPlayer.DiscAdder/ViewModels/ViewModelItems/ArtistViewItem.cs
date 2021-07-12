using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal abstract class ArtistViewItem
	{
		public abstract string Title { get; }

		public abstract ArtistModel ArtistModel { get; }

		public override string ToString()
		{
			return Title;
		}
	}
}
