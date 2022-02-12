using System;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class SpecificInputArtistItem : BasicInputArtistItem
	{
		public bool IsNew { get; }

		public override string ViewTitle => ArtistName;

		public override string ArtistName { get; }

		public SpecificInputArtistItem(string artistName, bool isNewArtist)
		{
			ArtistName = artistName ?? throw new ArgumentNullException(nameof(artistName));
			IsNew = isNewArtist;
		}

		public bool Equals(SpecificInputArtistItem otherArtist)
		{
			return Matches(otherArtist.ArtistName);
		}

		public bool Matches(string otherArtistName)
		{
			return String.Equals(ArtistName, otherArtistName, StringComparison.Ordinal);
		}
	}
}
