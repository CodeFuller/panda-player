using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public sealed class CompilationDiscWithArtistInfoViewItem : CompilationDiscViewItem
	{
		public override string DiscTypeTitle => "Compilation with Artists";

		public override Artist Artist
		{
			get { return null; }
			set
			{
				throw new InvalidOperationException(Current($"Artist could not be set for '{Title}' directory"));
			}
		}
		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		public CompilationDiscWithArtistInfoViewItem(string sourcePath, AddedDiscInfo discInfo, IEnumerable<Artist> availableArtists, 
			Uri destinationUri, IEnumerable<Genre> availableGenres) : base(sourcePath, discInfo, availableArtists, destinationUri, availableGenres)
		{
		}

		public override Artist GetSongArtist(AddedSongInfo song)
		{
			return LookupArtist(song.Artist);
		}
	}
}
