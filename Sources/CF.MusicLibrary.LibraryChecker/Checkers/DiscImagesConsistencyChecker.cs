using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Common.Images;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;
using CF.MusicLibrary.LibraryChecker.Registrators;
using Microsoft.Extensions.Logging;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class DiscImagesConsistencyChecker : IDiscImagesConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IImageInfoProvider imageInfoProvider;
		private readonly IDiscImageValidator discImageValidator;
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly ICheckScope checkScope;
		private readonly ILogger<DiscImagesConsistencyChecker> logger;

		public DiscImagesConsistencyChecker(IMusicLibrary musicLibrary, IImageInfoProvider imageInfoProvider, IDiscImageValidator discImageValidator,
			ILibraryInconsistencyRegistrator inconsistencyRegistrator, ICheckScope checkScope, ILogger<DiscImagesConsistencyChecker> logger)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.imageInfoProvider = imageInfoProvider ?? throw new ArgumentNullException(nameof(imageInfoProvider));
			this.discImageValidator = discImageValidator ?? throw new ArgumentNullException(nameof(discImageValidator));
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.checkScope = checkScope ?? throw new ArgumentNullException(nameof(checkScope));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckDiscImagesConsistency(IEnumerable<Disc> discs, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking discs images consistency...");

			foreach (var disc in discs.Where(checkScope.Contains))
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
				inconsistencyRegistrator.RegisterDiscCoverIsTooSmall(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.ImageIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterDiscCoverIsTooBig(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.FileSizeIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterImageFileIsTooBig(disc, imageInfo);
			}

			if ((validationResults & ImageValidationResults.UnsupportedFormat) != 0)
			{
				inconsistencyRegistrator.RegisterImageHasUnsupportedFormat(disc, imageInfo);
			}
		}
	}
}
