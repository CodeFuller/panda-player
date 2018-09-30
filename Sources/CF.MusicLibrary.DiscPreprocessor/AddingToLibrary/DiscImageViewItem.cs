using System;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class DiscImageViewItem : ViewModelBase
	{
		private readonly IImageFile imageFile;

		public Disc Disc { get; }

		public Uri DiscUri => Disc.Uri;

		public DiscImageType ImageType { get; }

		public bool ImageIsValid => imageFile.ImageIsValid;

		public string ImageStatus => ImageIsValid ? imageFile.ImageProperties : imageFile.ImageStatus;

		public ImageInfo ImageInfo => imageFile.ImageInfo;

		public DiscImageViewItem(Disc disc, DiscImageType imageType, IImageFile imageFile)
		{
			Disc = disc;
			ImageType = imageType;
			this.imageFile = imageFile;
		}
	}
}
