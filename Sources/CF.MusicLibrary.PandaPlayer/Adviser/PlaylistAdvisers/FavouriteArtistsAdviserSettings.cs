using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class FavouriteArtistsAdviserSettings
	{
		public int PlaybacksBetweenFavouriteArtistDiscs { get; set; }

		public IReadOnlyCollection<string> FavouriteArtists { get; set; } = new Collection<string>();
	}
}
