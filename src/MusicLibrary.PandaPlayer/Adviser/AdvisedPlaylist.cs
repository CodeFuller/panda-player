using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser
{
	internal class AdvisedPlaylist
	{
		private readonly DiscModel disc;

		public AdvisedPlaylistType AdvisedPlaylistType { get; private init; }

		public string Title { get; private init; }

		public IReadOnlyCollection<SongModel> Songs { get; private init; }

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

			private init => disc = value;
		}

		private AdvisedPlaylist()
		{
		}

		public static AdvisedPlaylist ForDisc(DiscModel disc)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.Disc,
				Title = FormatDiscTitle(disc),
				Songs = disc.ActiveSongs.ToList(),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForFavoriteArtistDisc(DiscModel disc)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.FavoriteArtistDisc,
				Title = "*** " + FormatDiscTitle(disc),
				Songs = disc.ActiveSongs.ToList(),
				Disc = disc,
			};
		}

		public static AdvisedPlaylist ForHighlyRatedSongs(IEnumerable<SongModel> songs)
		{
			return new()
			{
				AdvisedPlaylistType = AdvisedPlaylistType.HighlyRatedSongs,
				Title = "Highly Rated Songs",
				Songs = songs.ToList(),
			};
		}

		private static string FormatDiscTitle(DiscModel disc)
		{
			return $"{disc.Folder.Name} / {disc.Title}";
		}
	}
}
