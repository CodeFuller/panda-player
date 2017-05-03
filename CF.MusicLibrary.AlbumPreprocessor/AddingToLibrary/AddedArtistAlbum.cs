using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public sealed class AddedArtistAlbum : AddedAlbum
	{
		private readonly AlbumInfo albumInfo;

		public override AlbumType AlbumsType => AlbumType.ArtistAlbum;

		public override string AlbumTypeTitle => "Artist Album";

		public override string Artist
		{
			get { return albumInfo.Artist; }
			set
			{
				throw new InvalidOperationException(Current($"Artist could not be changed for '{Title}' directory"));
			}
		}
		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		public override int? Year
		{
			get { return albumInfo.Year; }
			set
			{
				throw new InvalidOperationException(Current($"Year could not be set for '{Title}' directory"));
			}
		}

		public override bool DestinationUriIsEditable => true;

		private readonly List<Uri> availableDestinationUris;
		public override IEnumerable<Uri> AvailableDestinationUris => availableDestinationUris;

		public override bool RequiredDataIsFilled => !(Genre == null || DestinationUriIsNotFilled);

		public string NameInStorage => albumInfo.NameInStorage;

		public AddedArtistAlbum(string sourcePath, AlbumInfo album,
			IEnumerable<Uri> availableDestinationUris, Uri destinationUri,
			IEnumerable<Genre> availableGenres, Genre genre) :
			base(sourcePath, album, availableGenres)
		{
			albumInfo = album;
			Genre = genre;
			this.availableDestinationUris = availableDestinationUris.ToList();
			DestinationUri = destinationUri;

			//	Should we keep Artist parsed from songs or should we clear it?
			//	Currently artist is parsed from the song filename by following regex: (.+) - (.+)
			//	It works for titles like 'Aerosmith - I Don't Want To Miss A Thing.mp3' but doesn't
			//	work for '09 - Lappi - I. Eramaajarvi.mp3'
			//	Here we determine whether major part of album songs has artist in title.
			//	If not then we clear Artist in all songs that have it.
			if (Songs.Count(s => String.IsNullOrEmpty(s.Artist)) > Songs.Count(s => !String.IsNullOrEmpty(s.Artist)))
			{
				foreach (var song in Songs)
				{
					song.DismissArtistInfo();
				}
			}
		}

		public override string GetSongArtist(SongInfo song)
		{
			if (song == null)
			{
				throw new ArgumentNullException(nameof(song));
			}

			return String.IsNullOrEmpty(song.Artist) ? Artist : song.Artist;
		}
	}
}
