using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class VariousArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Various>";

		public override ArtistModel ArtistModel => null;
	}
}
