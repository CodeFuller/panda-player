using System;
using System.Collections.Generic;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class NewDiscViewItem : DiscViewItem
	{
		public override string AlbumTitle
		{
			get { return Disc.AlbumTitle; }
			set
			{
				Disc.AlbumTitle = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(WarnAboutUnequalAlbumTitle));
			}
		}
		public override bool AlbumTitleIsEditable => true;

		private short? year;
		public override short? Year
		{
			get { return year; }
			set
			{
				Set(ref year, value);
				RaisePropertyChanged(nameof(WarnAboutNotFilledYear));
			}
		}
		public override bool YearIsEditable => true;

		public override bool RequiredDataIsFilled => !GenreIsNotFilled;

		protected NewDiscViewItem(AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			if (String.IsNullOrWhiteSpace(disc.Title))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			Disc = new Disc
			{
				Title = disc.Title,
				AlbumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(disc.Title),
				Uri = disc.UriWithinStorage,
			};
		}
	}
}
