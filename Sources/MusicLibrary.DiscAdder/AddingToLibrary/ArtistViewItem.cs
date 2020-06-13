using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
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
