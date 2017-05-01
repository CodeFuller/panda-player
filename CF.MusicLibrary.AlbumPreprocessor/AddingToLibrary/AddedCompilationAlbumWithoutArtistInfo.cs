using System;
using System.Collections.Generic;
using CF.MusicLibrary.AlbumPreprocessor.MusicStorage;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary
{
	public sealed class AddedCompilationAlbumWithoutArtistInfo : AddedCompilationAlbum
	{
		public override AlbumType AlbumsType => AlbumType.CompilationAlbumWithoutArtistInfo;

		public override string AlbumTypeTitle => "Compilation without Artists";

		private string artist;
		public override string Artist
		{
			get { return artist; }
			set
			{
				Set(ref artist, value);
				RaisePropertyChanged(nameof(ArtistIsNotFilled));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}
		}
		public override bool ArtistIsEditable => true;
		public override bool ArtistIsNotFilled => String.IsNullOrEmpty(Artist);

		public AddedCompilationAlbumWithoutArtistInfo(string sourcePath, AlbumInfo albumInfo, Uri destinationUri, IEnumerable<Genre> availableGenres) :
			base(sourcePath, albumInfo, destinationUri, availableGenres)
		{
		}

		public override string GetSongArtist(SongInfo song)
		{
			return Artist;
		}
	}
}
