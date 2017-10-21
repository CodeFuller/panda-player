using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Common.Images
{
	public interface IDiscImageValidator
	{
		ImageValidationResults ValidateDiscCoverImage(ImageInfo imageInfo);

		IEnumerable<string> GetValidationResultsHints(ImageValidationResults results);

		bool IsSupportedFileFormat(string fileName);
	}
}
