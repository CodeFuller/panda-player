using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class DiscImagesConsistencyChecker : IDiscImagesConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IImageInfoProvider imageInfoProvider;
		private readonly IDiscImageValidator discImageValidator;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		
		public DiscImagesConsistencyChecker(IMusicLibrary musicLibrary, IImageInfoProvider imageInfoProvider,
			IDiscImageValidator discImageValidator, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (imageInfoProvider == null)
			{
				throw new ArgumentNullException(nameof(imageInfoProvider));
			}
			if (discImageValidator == null)
			{
				throw new ArgumentNullException(nameof(discImageValidator));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicLibrary = musicLibrary;
			this.imageInfoProvider = imageInfoProvider;
			this.discImageValidator = discImageValidator;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckDiscImagesConsistency(IEnumerable<Disc> discs)
		{
			Logger.WriteInfo("Checking discs images consistency ...");

			foreach (var disc in discs)
			{
				await CheckDiscImagesConsistency(disc);
			}
		}

		private async Task CheckDiscImagesConsistency(Disc disc)
		{
			var discCoverImageFile = await musicLibrary.GetDiscCoverImage(disc);
			if (discCoverImageFile == null)
			{
				return;
			}

			var imageInfo = imageInfoProvider.GetImageInfo(discCoverImageFile);
			var validationResults = discImageValidator.ValidateDiscCoverImage(imageInfo);
			if (validationResults == ImageValidationResults.ImageIsOk)
			{
				return;
			}

			RegisterInconsistencies(disc, imageInfo, validationResults);
		}

		private void RegisterInconsistencies(Disc disc, ImageInfo imageInfo, ImageValidationResults validationResults)
		{
			if ((validationResults & ImageValidationResults.ImageIsTooSmall) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooSmall(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.ImageIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooBig(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.FileSizeIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_ImageFileIsTooBig(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.UnsupportedFormat) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_ImageHasUnsupportedFormat(disc, imageInfo);
			}
		}
	}
}
