using System.Collections.Generic;

namespace PandaPlayer.Shared.Images
{
	public interface IDiscImageValidator
	{
		ImageValidationResults ValidateDiscCoverImage(ImageInfo imageInfo);

		IEnumerable<string> GetValidationResultsHints(ImageValidationResults results);

		bool IsSupportedFileFormat(string fileName);
	}
}
