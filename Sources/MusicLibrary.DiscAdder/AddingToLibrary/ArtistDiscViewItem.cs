using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal sealed class ArtistDiscViewItem : NewDiscViewItem
	{
		public override string DiscTypeTitle => "Artist Disc";

		private readonly ArtistModel artist;

		public override ArtistModel Artist
		{
			get => artist;
			set => throw new InvalidOperationException(Current($"Artist could not be changed for '{DiscTitle}' disc"));
		}

		public override bool ArtistIsEditable => false;

		public override bool ArtistIsNotFilled => false;

		public override bool YearIsEditable => false;

		public ArtistDiscViewItem(AddedDiscInfo disc, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres, GenreModel genre)
			: base(disc, folderExists, availableArtists, availableGenres)
		{
			artist = LookupArtist(disc.Artist);
			Genre = LookupGenre(AvailableGenres, genre);
			Year = disc.Year;

			// Should we keep Artist parsed from songs or should we clear it?
			// Currently artist is parsed from the song filename by following regex: (.+) - (.+)
			// It works for titles like 'Aerosmith - I Don't Want To Miss A Thing.mp3' but doesn't
			// work for '09 - Lappi - I. Eramaajarvi.mp3'
			// Here we determine whether major part of disc songs has artist in title.
			// If not, then we clear Artist in all songs that have it.
			if (SourceSongs.Count(s => String.IsNullOrEmpty(s.Artist)) > SourceSongs.Count(s => !String.IsNullOrEmpty(s.Artist)))
			{
				foreach (var song in SourceSongs)
				{
					song.DismissArtistInfo();
				}
			}
		}

		protected override ArtistModel GetSongArtist(AddedSongInfo song)
		{
			return String.IsNullOrEmpty(song.Artist) ? Artist : LookupArtist(song.Artist);
		}

		private static GenreModel LookupGenre(IEnumerable<GenreModel> availableGenres, GenreModel genreModel)
		{
			if (genreModel == null)
			{
				return null;
			}

			var foundGenre = availableGenres.SingleOrDefault(g => new GenreEqualityComparer().Equals(g, genreModel));
			if (foundGenre == null)
			{
				throw new InvalidOperationException($"Failed to find genre '{genreModel.Name}' in the list of available genres");
			}

			return foundGenre;
		}
	}
}
