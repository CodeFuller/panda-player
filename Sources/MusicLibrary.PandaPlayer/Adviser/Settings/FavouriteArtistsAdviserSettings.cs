using System.Collections.Generic;

namespace MusicLibrary.PandaPlayer.Adviser.Settings
{
	internal class FavouriteArtistsAdviserSettings
	{
		public int PlaybacksBetweenFavouriteArtistDiscs { get; set; }

		public IReadOnlyCollection<string> FavouriteArtists { get; set; }
	}
}
