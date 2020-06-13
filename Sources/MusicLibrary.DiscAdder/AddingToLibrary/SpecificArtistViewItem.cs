using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class SpecificArtistViewItem : ArtistViewItem
	{
		public override string Title => ArtistModel.Name;

		public override ArtistModel ArtistModel { get; }

		public bool IsNew => ArtistModel.Id == null;

		public SpecificArtistViewItem(ArtistModel artist)
		{
			ArtistModel = artist ?? throw new ArgumentNullException(nameof(artist));
		}

		public bool Equals(SpecificArtistViewItem cmp)
		{
			return Matches(cmp.ArtistModel.Name);
		}

		public bool Matches(string artistName)
		{
			return String.Equals(ArtistModel.Name, artistName, StringComparison.Ordinal);
		}
	}
}
