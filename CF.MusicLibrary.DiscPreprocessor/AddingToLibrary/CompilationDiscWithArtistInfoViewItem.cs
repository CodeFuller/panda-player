using System;
using System.Collections.Generic;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core.Objects;
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
				throw new InvalidOperationException(Current($"Artist could not be set for '{DiscTitle}' disc"));
			}
		}
		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		public CompilationDiscWithArtistInfoViewItem(IDiscArtImageFile discArtImageFile, AddedDiscInfo discInfo, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(discArtImageFile, discInfo, availableArtists, availableGenres)
		{
		}

		protected override Artist GetSongArtist(AddedSongInfo song)
		{
			return LookupArtist(song.Artist);
		}
	}
}
