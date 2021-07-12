using System.Collections.Generic;

namespace PandaPlayer.Adviser.Settings
{
	internal class FavoriteArtistsAdviserSettings
	{
		public int PlaybacksBetweenFavoriteArtistDiscs { get; set; }

		public IReadOnlyCollection<string> FavoriteArtists { get; set; }
	}
}
