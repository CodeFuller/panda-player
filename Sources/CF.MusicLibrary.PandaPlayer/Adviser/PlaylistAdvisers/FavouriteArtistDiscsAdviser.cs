using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public class FavouriteArtistDiscsAdviser : IPlaylistAdviser
	{
		private readonly IPlaylistAdviser discAdviser;

		public FavouriteArtistDiscsAdviser(IPlaylistAdviser discAdviser)
		{
			this.discAdviser = discAdviser ?? throw new ArgumentNullException(nameof(discAdviser));
		}

		public IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			return discAdviser.Advise(discLibrary)
				.Where(a => a.Disc.Artist != null && a.Disc.Artist.IsFavourite)
				.Select(a => AdvisedPlaylist.ForFavouriteArtistDisc(a.Disc));
		}
	}
}
