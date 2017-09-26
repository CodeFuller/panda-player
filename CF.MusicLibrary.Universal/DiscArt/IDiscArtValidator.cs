﻿using System.Collections.Generic;

namespace CF.MusicLibrary.Universal.DiscArt
{
	public interface IDiscArtValidator
	{
		DiscArtImageInfo GetImageInfo(string imageFileName);

		DiscArtValidationResults ValidateDiscCoverImage(string imageFileName);

		DiscArtValidationResults ValidateDiscCoverImage(DiscArtImageInfo imageInfo);

		IEnumerable<string> GetValidationResultsHints(DiscArtValidationResults results);
	}
}
