using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.LibraryChecker.Registrators;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public class DiscArtConsistencyChecker : IDiscArtConsistencyChecker
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly IDiscArtValidator discArtValidator;

		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		
		public DiscArtConsistencyChecker(IMusicLibrary musicLibrary, IDiscArtValidator discArtValidator, ILibraryInconsistencyRegistrator inconsistencyRegistrator)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (discArtValidator == null)
			{
				throw new ArgumentNullException(nameof(discArtValidator));
			}
			if (inconsistencyRegistrator == null)
			{
				throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			}

			this.musicLibrary = musicLibrary;
			this.discArtValidator = discArtValidator;
			this.inconsistencyRegistrator = inconsistencyRegistrator;
		}

		public async Task CheckDiscArtsConsistency(IEnumerable<Disc> discs)
		{
			Logger.WriteInfo("Checking discs arts consistency ...");

			foreach (var disc in discs)
			{
				await CheckDiscArtConsistency(disc);
			}
		}

		private async Task CheckDiscArtConsistency(Disc disc)
		{
			var discCoverImageFile = await musicLibrary.GetDiscCoverImage(disc);
			if (discCoverImageFile == null)
			{
				return;
			}

			var imageInfo = discArtValidator.GetImageInfo(discCoverImageFile);
			var validationResults = discArtValidator.ValidateDiscCoverImage(imageInfo);
			if (validationResults == DiscArtValidationResults.ImageIsOk)
			{
				return;
			}

			RegisterInconsistencies(disc, imageInfo, validationResults);
		}

		private void RegisterInconsistencies(Disc disc, DiscArtImageInfo imageInfo, DiscArtValidationResults validationResults)
		{
			if ((validationResults & DiscArtValidationResults.ImageIsTooSmall) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooSmall(disc, imageInfo);
			}

			if ((validationResults & DiscArtValidationResults.ImageIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_DiscCoverIsTooBig(disc, imageInfo);
			}

			if ((validationResults & DiscArtValidationResults.FileSizeIsTooBig) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_ImageFileIsTooBig(disc, imageInfo);
			}

			if ((validationResults & DiscArtValidationResults.UnsupportedFormat) != 0)
			{
				inconsistencyRegistrator.RegisterInconsistency_ImageHasUnsupportedFormat(disc, imageInfo);
			}
		}
	}
}
