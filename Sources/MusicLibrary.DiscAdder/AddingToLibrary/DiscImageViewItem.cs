using GalaSoft.MvvmLight;
using MusicLibrary.Core.Models;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.DiscAdder.AddingToLibrary
{
	internal class DiscImageViewItem : ViewModelBase
	{
		private readonly IImageFile imageFile;

		public DiscModel Disc { get; }

		public string SourceImageFilePath => ImageInfo.FileName;

		public DiscImageType ImageType { get; }

		public bool ImageIsValid => imageFile.ImageIsValid;

		public string ImageStatus => ImageIsValid ? imageFile.ImageProperties : imageFile.ImageStatus;

		public ImageInfo ImageInfo => imageFile.ImageInfo;

		public DiscImageViewItem(DiscModel disc, DiscImageType imageType, IImageFile imageFile)
		{
			Disc = disc;
			ImageType = imageType;
			this.imageFile = imageFile;
		}
	}
}
