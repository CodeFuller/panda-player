using System;
using System.Collections.Generic;
using MusicLibrary.Core.Objects;
using MusicLibrary.DiscPreprocessor.MusicStorage;

namespace MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class NewDiscViewItem : DiscViewItem
	{
		public override string AlbumTitle
		{
			get => Disc.AlbumTitle;
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
			get => year;
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
			if (String.IsNullOrWhiteSpace(disc.DiscTitle))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			Disc = new Disc
			{
				Title = disc.DiscTitle,
				AlbumTitle = disc.AlbumTitle,
				Uri = disc.UriWithinStorage,
			};
		}
	}
}
