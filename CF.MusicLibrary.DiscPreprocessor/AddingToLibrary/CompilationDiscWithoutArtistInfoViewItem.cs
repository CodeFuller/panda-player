using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public sealed class CompilationDiscWithoutArtistInfoViewItem : CompilationDiscViewItem
	{
		public override string DiscTypeTitle => "Compilation without Artists";

		private Artist artist;
		public override Artist Artist
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
		public override bool ArtistIsNotFilled => Artist == null;

		public CompilationDiscWithoutArtistInfoViewItem(string sourcePath, AddedDiscInfo discInfo, IEnumerable<Artist> availableArtists,
			Uri destinationUri, IEnumerable<Genre> availableGenres) : base(sourcePath, discInfo, availableArtists, destinationUri, availableGenres)
		{
		}

		public override Artist GetSongArtist(AddedSongInfo song)
		{
			return Artist;
		}
	}
}
