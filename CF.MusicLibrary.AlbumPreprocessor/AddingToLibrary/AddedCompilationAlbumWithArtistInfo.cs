using System;
using System.Collections.Generic;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public sealed class AddedCompilationAlbumWithArtistInfo : AddedCompilationAlbum
	{
		public override AlbumType AlbumsType => AlbumType.CompilationAlbumWithArtistInfo;

		public override string AlbumTypeTitle => "Compilation with Artists";

		public override string Artist
		{
			get { return null; }
			set
			{
				throw new InvalidOperationException(Current($"Artist could not be set for '{Title}' directory"));
			}
		}
		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		public AddedCompilationAlbumWithArtistInfo(string sourcePath, AlbumInfo albumInfo, Uri destinationUri, IEnumerable<Genre> availableGenres) :
			base(sourcePath, albumInfo, destinationUri, availableGenres)
		{
		}

		public override string GetSongArtist(SongInfo song)
		{
			if (song == null)
			{
				throw new ArgumentNullException(nameof(song));
			}

			return song.Artist;
		}
	}
}
