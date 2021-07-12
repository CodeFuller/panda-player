using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class EmptyArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Empty>";

		public override ArtistModel ArtistModel => null;
	}
}
