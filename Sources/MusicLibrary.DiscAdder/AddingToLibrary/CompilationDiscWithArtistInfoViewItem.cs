using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal sealed class CompilationDiscWithArtistInfoViewItem : CompilationDiscViewItem
	{
		public override string DiscTypeTitle => "Compilation with Artists";

		public override ArtistModel Artist
		{
			get => null;
			set => throw new InvalidOperationException(Current($"Artist could not be set for '{DiscTitle}' disc"));
		}

		public override bool ArtistIsEditable => false;

		public override bool ArtistIsNotFilled => false;

		public CompilationDiscWithArtistInfoViewItem(AddedDiscInfo discInfo, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(discInfo, folderExists, availableArtists, availableGenres)
		{
		}

		protected override ArtistModel GetSongArtist(AddedSongInfo song)
		{
			return LookupArtist(song.Artist);
		}
	}
}
