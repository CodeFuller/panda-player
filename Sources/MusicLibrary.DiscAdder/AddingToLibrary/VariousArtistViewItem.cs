using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class VariousArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Various>";

		public override ArtistModel ArtistModel => null;
	}
}
