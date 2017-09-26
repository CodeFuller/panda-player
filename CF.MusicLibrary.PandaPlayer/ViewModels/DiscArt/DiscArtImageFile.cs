using System;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Universal.DiscArt;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.DiscArt
{
	internal class DiscArtImageFile : ViewModelBase, IDiscArtImageFile
	{
		private readonly IDiscArtValidator discArtValidator;
		private readonly IFileSystemFacade fileSystemFacade;

		private string imageFileName;
		public string ImageFileName
		{
			get { return imageFileName; }
			private set
			{
				Set(ref imageFileName, value);
				RaisePropertyChanged(nameof(ImageIsValid));
				RaisePropertyChanged(nameof(ImageProperties));
				RaisePropertyChanged(nameof(ImageStatus));
			}
		}

		public bool IsTemporaryFile { get; private set; }

		public DiscArtImageInfo ImageInfo { get; private set; }

		public bool ImageIsValid => ImageInfo != null && discArtValidator.ValidateDiscCoverImage(ImageInfo) == DiscArtValidationResults.ImageIsOk;

		public string ImageProperties => ImageInfo != null
			? Current($"{ImageInfo.Width} x {ImageInfo.Height}, {FileSizeFormatter.GetFormattedFileSize(ImageInfo.FileSize)}, {ImageInfo.FormatName}")
			: "N/A";

		public string ImageStatus
		{
			get
			{
				if (ImageInfo == null)
				{
					return "Image is not set";
				}

				var validationResults = discArtValidator.ValidateDiscCoverImage(ImageInfo);
				return String.Join("; ", discArtValidator.GetValidationResultsHints(validationResults));
			}
		}

		public DiscArtImageFile(IDiscArtValidator discArtValidator, IFileSystemFacade fileSystemFacade)
		{
			if (discArtValidator == null)
			{
				throw new ArgumentNullException(nameof(discArtValidator));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.discArtValidator = discArtValidator;
			this.fileSystemFacade = fileSystemFacade;
		}

		public void Load(string fileName, bool isTemporaryFile)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			Clean();

			ImageInfo = discArtValidator.GetImageInfo(fileName);
			ImageFileName = fileName;
			IsTemporaryFile = isTemporaryFile;
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
			ImageFileName = null;
			IsTemporaryFile = false;
		}
	}
}
