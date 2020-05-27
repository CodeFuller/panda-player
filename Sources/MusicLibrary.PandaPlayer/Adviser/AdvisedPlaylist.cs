using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.Adviser
{
	internal class AdvisedPlaylist
	{
		public AdvisedPlaylistType AdvisedPlaylistType { get; private set; }

		public string Title { get; private set; }

		public IReadOnlyCollection<SongModel> Songs { get; private set; }

		private DiscModel disc;

		public DiscModel Disc
		{
			get
			{
				if (disc == null)
				{
					throw new InvalidOperationException("Advise does not have a disc");
				}

				return disc;
			}

			private set => disc = value;
		}

		private AdvisedPlaylist()
		{
		}

		public static AdvisedPlaylist ForDisc(DiscModel disc)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.Disc,
				Title = FormatDiscTitle(disc),
				Songs = new List<SongModel>(disc.Songs),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForFavouriteArtistDisc(DiscModel disc)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.FavouriteArtistDisc,
				Title = "*** " + FormatDiscTitle(disc),
				Songs = new List<SongModel>(disc.Songs),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForHighlyRatedSongs(IEnumerable<SongModel> songs)
		{
			return new AdvisedPlaylist
			{
				AdvisedPlaylistType = AdvisedPlaylistType.HighlyRatedSongs,
				Title = "Highly Rated Songs",
				Songs = songs.ToList(),
			};
		}

		private static string FormatDiscTitle(DiscModel disc)
		{
			return Current($"{disc.Folder.Name} / {disc.Title}");
		}
	}
}
