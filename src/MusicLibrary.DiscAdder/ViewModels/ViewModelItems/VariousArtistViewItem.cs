using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.ViewModels.ViewModelItems
{
	internal class VariousArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Various>";

		public override ArtistModel ArtistModel => null;
	}
}
