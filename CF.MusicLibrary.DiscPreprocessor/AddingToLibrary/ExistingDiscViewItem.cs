using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class ExistingDiscViewItem : DiscViewItem
	{
		public override string DiscTypeTitle => "Existing Disc";
		public override bool WarnAboutDiscType => true;

		public override string AlbumTitle
		{
			get { return Disc.AlbumTitle; }
			set
			{
				throw new InvalidOperationException(Current($"Album title could not be changed for '{DiscTitle}' disc"));
			}
		}
		public override bool AlbumTitleIsEditable => false;

		public override Artist Artist
		{
			get { return Disc.Artist; }
			set
			{
				throw new InvalidOperationException(Current($"Artist could not be changed for '{DiscTitle}' disc"));
			}
		}

		public override bool ArtistIsEditable => false;
		public override bool ArtistIsNotFilled => false;

		public override short? Year
		{
			get { return Disc.Year; }
			set
			{
				throw new InvalidOperationException(Current($"Year could not be changed for '{DiscTitle}' disc"));
			}
		}
		public override bool YearIsEditable => false;

		public override bool RequiredDataIsFilled => true;

		public ExistingDiscViewItem(Disc existingDisc, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			Disc = existingDisc;
			Genre = existingDisc.Genre;
		}

		protected override Artist GetSongArtist(AddedSongInfo song)
		{
			return String.IsNullOrEmpty(song.Artist) ? Disc.Artist : LookupArtist(song.Artist);
		}
	}
}
