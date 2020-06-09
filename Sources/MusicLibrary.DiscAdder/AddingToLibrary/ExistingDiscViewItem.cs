using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.Extensions;
using MusicLibrary.DiscAdder.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class ExistingDiscViewItem : DiscViewItem
	{
		public override string DiscTypeTitle => "Existing Disc";

		public override bool WarnAboutDiscType => true;

		public override string AlbumTitle
		{
			get => Disc.AlbumTitle;
			set => throw new InvalidOperationException(Current($"Album title could not be changed for '{DiscTitle}' disc"));
		}

		public override bool AlbumTitleIsEditable => false;

		public override ArtistModel Artist
		{
			get => Disc.SoloArtist;
			set => throw new InvalidOperationException(Current($"Artist could not be changed for '{DiscTitle}' disc"));
		}

		public override bool ArtistIsEditable => false;

		public override bool ArtistIsNotFilled => false;

		public override bool YearIsEditable => false;

		public override bool RequiredDataIsFilled => true;

		public ExistingDiscViewItem(DiscModel existingDisc, AddedDiscInfo disc, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(disc, folderExists: true, availableArtists, availableGenres)
		{
			Disc = existingDisc;
			Genre = existingDisc.GetGenre();
		}

		protected override ArtistModel GetSongArtist(AddedSongInfo song)
		{
			return String.IsNullOrEmpty(song.Artist) ? Disc.SoloArtist : LookupArtist(song.Artist);
		}
	}
}
