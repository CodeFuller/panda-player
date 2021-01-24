using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.ViewModels.ViewModelItems
{
	internal class EmptyArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Empty>";

		public override ArtistModel ArtistModel => null;
	}
}
