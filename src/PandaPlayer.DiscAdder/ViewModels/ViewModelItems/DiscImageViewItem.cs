using System;
using GalaSoft.MvvmLight;
using PandaPlayer.Core.Models;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.DiscAdder.ViewModels.ViewModelItems
{
	internal class DiscImageViewItem : ViewModelBase
	{
		private readonly IImageFile imageFile;

		public string DiscSourcePath { get; }

		public string SourceImageFilePath => ImageInfo?.FileName;

		public DiscImageType ImageType { get; }

		public bool ImageIsValid => imageFile.ImageIsValid;

		public string ImageStatus => ImageIsValid ? imageFile.ImageProperties : imageFile.ImageStatus;

		public ImageInfo ImageInfo => imageFile.ImageInfo;

		public DiscImageViewItem(DiscViewItem discItem, DiscImageType imageType, IImageFile imageFile)
		{
			DiscSourcePath = discItem?.SourcePath ?? throw new ArgumentNullException(nameof(discItem));
			ImageType = imageType;
			this.imageFile = imageFile;
		}
	}
}
