using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser
{
	public class AdvisedPlaylist
	{
		public AdvisedPlaylistType AdvisedPlaylistType { get; private set; }

		public string Title { get; private set; }

		public IReadOnlyCollection<Song> Songs { get; private set; }

		private Disc disc;
		public Disc Disc
		{
			get
			{
				if (disc == null)
				{
					throw new InvalidOperationException("Advise does not have a disc");
				}

				return disc;
			}

			private set { disc = value; }
		}

		private AdvisedPlaylist()
		{
		}

		public static AdvisedPlaylist ForDisc(Disc disc)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.Disc,
				Title = disc.Artist == null ? disc.Title : FormattableStringExtensions.Current($"{disc.Artist.Name} - {disc.Title}"),
				Songs = new List<Song>(disc.Songs),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForFavouriteArtistDisc(Disc disc)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.FavouriteArtistDisc,
				Title = "*** " + (disc.Artist == null ? disc.Title : FormattableStringExtensions.Current($"{disc.Artist.Name} - {disc.Title}")),
				Songs = new List<Song>(disc.Songs),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForHighlyRatedSongs(IEnumerable<Song> songs)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.HighlyRatedSongs,
				Title = "Highly Rated Songs",
				Songs = songs.ToList(),
			};
		}
	}
}
