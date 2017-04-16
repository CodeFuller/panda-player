using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	public class ArtistLibraryBuilder : IArtistLibraryBuilder
	{
		private readonly IDiscArtistGroupper discArtistGroupper;

		public ArtistLibraryBuilder(IDiscArtistGroupper discArtistGroupper)
		{
			if (discArtistGroupper == null)
			{
				throw new ArgumentNullException(nameof(discArtistGroupper));
			}

			this.discArtistGroupper = discArtistGroupper;
		}

		public ArtistLibrary Build(DiscLibrary discLibrary)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}

			List<LibraryArtist> artists = new List<LibraryArtist>();

			foreach (var disc in discLibrary)
			{
				LibraryArtist discArtist = discArtistGroupper.GetDiscArtist(disc);

				var libraryArtist = artists.SingleOrDefault(a => a.Id == discArtist.Id);
				if (libraryArtist == null)
				{
					libraryArtist = discArtist;
					artists.Add(libraryArtist);
				}

				libraryArtist.AddDisc(new LibraryDisc(libraryArtist, disc));
			}

			return new ArtistLibrary(artists);
		}
	}
}
