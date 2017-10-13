using System;
using System.Collections.Generic;
using System.ComponentModel;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public abstract class NewDiscViewItem : DiscViewItem
	{
		private readonly IDiscArtImageFile discArtImageFile;

		public override string DiscTitle { get; }

		private string albumTitle;
		public override string AlbumTitle
		{
			get { return albumTitle; }
			set
			{
				Set(ref albumTitle, value);
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

		public override bool DiscArtIsValid => discArtImageFile.ImageIsValid;

		public override string DiscArtInfo => DiscArtIsValid ? discArtImageFile.ImageProperties : discArtImageFile.ImageStatus;

		public override bool RequiredDataIsFilled => !GenreIsNotFilled && DiscArtIsValid;

		protected override Disc Disc => new Disc
		{
			Title = DiscTitle,
			AlbumTitle = AlbumTitle,
			Uri = DestinationUri,
		};

		public override AddedDiscCoverImage AddedDiscCoverImage
		{
			get
			{
				if (!DiscArtIsValid)
				{
					throw new InvalidOperationException("Disc Art is not valid");
				}

				return new AddedDiscCoverImage(Disc, discArtImageFile.ImageFileName);
			}
		}

		protected NewDiscViewItem(IDiscArtImageFile discArtImageFile, AddedDiscInfo disc, IEnumerable<Artist> availableArtists, IEnumerable<Genre> availableGenres)
			: base(disc, availableArtists, availableGenres)
		{
			if (discArtImageFile == null)
			{
				throw new ArgumentNullException(nameof(discArtImageFile));
			}

			if (String.IsNullOrWhiteSpace(disc.Title))
			{
				throw new InvalidOperationException("Disc title could not be empty");
			}

			this.discArtImageFile = discArtImageFile;
			this.discArtImageFile.PropertyChanged += DiscArtImageFileOnPropertyChanged;

			DiscTitle = disc.Title;
			albumTitle = DiscTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(disc.Title);
		}

		private void DiscArtImageFileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IDiscArtImageFile.ImageIsValid))
			{
				RaisePropertyChanged(nameof(DiscArtIsValid));
				RaisePropertyChanged(nameof(DiscArtInfo));
				RaisePropertyChanged(nameof(RequiredDataIsFilled));
			}

			if (e.PropertyName == nameof(IDiscArtImageFile.ImageStatus) || e.PropertyName == nameof(IDiscArtImageFile.ImageProperties))
			{
				RaisePropertyChanged(nameof(DiscArtInfo));
			}
		}

		public override void SetDiscCoverImage(string imageFileName)
		{
			discArtImageFile.Load(imageFileName, false);
		}

		public override void UnsetDiscCoverImage()
		{
			discArtImageFile.Unload();
		}
	}
}
