using System;
using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.DiscAdder.MusicStorage;

namespace MusicLibrary.DiscAdder.AddingToLibrary
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

		public override int? Year
		{
			get => Disc.Year;
			set
			{
				Disc.Year = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(WarnAboutNotFilledYear));
			}
		}

		public override bool YearIsEditable => true;

		public override bool RequiredDataIsFilled => !GenreIsNotFilled;

		protected NewDiscViewItem(AddedDiscInfo disc, bool folderExists, IEnumerable<ArtistModel> availableArtists, IEnumerable<GenreModel> availableGenres)
			: base(disc, folderExists, availableArtists, availableGenres)
		{
			if (String.IsNullOrWhiteSpace(disc.DiscTitle))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			Disc = new DiscModel
			{
				Title = disc.DiscTitle,
				TreeTitle = disc.TreeTitle,
				AlbumTitle = disc.AlbumTitle,
			};
		}
	}
}
