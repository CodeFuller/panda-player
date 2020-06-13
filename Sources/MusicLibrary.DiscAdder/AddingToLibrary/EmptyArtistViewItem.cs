using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class EmptyArtistViewItem : ArtistViewItem
	{
		public override string Title => "<Empty>";

		public override ArtistModel ArtistModel => null;
	}
}
