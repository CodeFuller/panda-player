using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public sealed class ArtistDiscViewItem : DiscViewItem
	{
		public override string DiscTypeTitle => "Artist Disc";

		private readonly Artist artist;
		public override Artist Artist
		{
			get { return artist; }
			set
			{
				throw new InvalidOperationException(Current($"Artist could not be changed for '{Title}' directory"));
			}
		}
		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		private readonly short? year;
		public override short? Year
		{
			get { return year; }
			set
			{
				throw new InvalidOperationException(Current($"Year could not be set for '{Title}' directory"));
			}
		}

		public override bool DestinationUriIsEditable => true;

		private readonly List<Uri> availableDestinationUris;
		public override IEnumerable<Uri> AvailableDestinationUris => availableDestinationUris;

		public override bool RequiredDataIsFilled => !(Genre == null || DestinationUriIsNotFilled);

		public string NameInStorage { get; private set; }

		public ArtistDiscViewItem(string sourcePath, AddedDiscInfo disc,
			IEnumerable<Artist> availableArtists,
			IEnumerable<Uri> availableDestinationUris, Uri destinationUri,
			IEnumerable<Genre> availableGenres, Genre genre) :
			base(sourcePath, disc, availableArtists, availableGenres)
		{
			artist = LookupArtist(disc.Artist);
			Genre = genre;
			year = disc.Year;
			this.availableDestinationUris = availableDestinationUris.ToList();
			DestinationUri = destinationUri;
			NameInStorage = disc.NameInStorage;

			//	Should we keep Artist parsed from songs or should we clear it?
			//	Currently artist is parsed from the song filename by following regex: (.+) - (.+)
			//	It works for titles like 'Aerosmith - I Don't Want To Miss A Thing.mp3' but doesn't
			//	work for '09 - Lappi - I. Eramaajarvi.mp3'
			//	Here we determine whether major part of disc songs has artist in title.
			//	If not then we clear Artist in all songs that have it.
			if (SourceSongs.Count(s => String.IsNullOrEmpty(s.Artist)) > SourceSongs.Count(s => !String.IsNullOrEmpty(s.Artist)))
			{
				foreach (var song in SourceSongs)
				{
					song.DismissArtistInfo();
				}
			}
		}

		public override Artist GetSongArtist(AddedSongInfo song)
		{
			return String.IsNullOrEmpty(song.Artist) ? Artist : LookupArtist(song.Artist);
		}
	}
}
