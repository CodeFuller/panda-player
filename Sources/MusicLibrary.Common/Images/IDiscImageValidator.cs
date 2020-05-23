using System.Collections.Generic;

namespace MusicLibrary.Common.Images
{
	internal interface IDiscImageValidator
	{
		ImageValidationResults ValidateDiscCoverImage(ImageInfo imageInfo);

		IEnumerable<string> GetValidationResultsHints(ImageValidationResults results);

		bool IsSupportedFileFormat(string fileName);
	}
}
