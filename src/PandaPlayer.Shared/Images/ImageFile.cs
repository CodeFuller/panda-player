using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaPlayer.Core.Facades;

namespace PandaPlayer.Shared.Images
{
	internal sealed class ImageFile : ObservableObject, IImageFile
	{
		private readonly IDiscImageValidator discImageValidator;
		private readonly IImageInfoProvider imageInfoProvider;
		private readonly IFileSystemFacade fileSystemFacade;

		public string ImageFileName => ImageInfo?.FileName;

		private bool IsTemporaryFile { get; set; }

		private ImageInfo imageInfo;

		public ImageInfo ImageInfo
		{
			get => imageInfo;
			private set
			{
				SetProperty(ref imageInfo, value);
				OnPropertyChanged(nameof(ImageFileName));
				OnPropertyChanged(nameof(ImageIsValid));
				OnPropertyChanged(nameof(ImageProperties));
				OnPropertyChanged(nameof(ImageStatus));
			}
		}

		public bool ImageIsValid => ImageInfo != null && discImageValidator.ValidateDiscCoverImage(ImageInfo) == ImageValidationResults.ImageIsOk;

		public string ImageProperties => ImageInfo != null
			? $"{ImageInfo.Width} x {ImageInfo.Height}, {FileSizeFormatter.GetFormattedFileSize(ImageInfo.FileSize)}, {ImageInfo.FormatName}"
			: "N/A";

		public string ImageStatus
		{
			get
			{
				if (ImageInfo == null)
				{
					return "Image is not set";
				}

				var validationResults = discImageValidator.ValidateDiscCoverImage(ImageInfo);
				return String.Join("; ", discImageValidator.GetValidationResultsHints(validationResults));
			}
		}

		public ImageFile(IDiscImageValidator discImageValidator, IImageInfoProvider imageInfoProvider, IFileSystemFacade fileSystemFacade)
		{
			this.discImageValidator = discImageValidator ?? throw new ArgumentNullException(nameof(discImageValidator));
			this.imageInfoProvider = imageInfoProvider ?? throw new ArgumentNullException(nameof(imageInfoProvider));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
		}

		public void Load(string fileName, bool isTemporaryFile)
		{
			if (ImageInfo != null)
			{
				Clean();
			}

			IsTemporaryFile = isTemporaryFile;
			ImageInfo = imageInfoProvider.GetImageInfo(fileName);
		}

		public void Unload()
		{
			Clean();
		}

		private void Clean()
		{
			if (ImageFileName != null && IsTemporaryFile)
			{
				fileSystemFacade.DeleteFile(ImageFileName);
			}

			ImageInfo = null;
			IsTemporaryFile = false;
		}
	}
}
