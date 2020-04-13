using System.Collections.Generic;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Common.Images
{
	public interface IDiscImageValidator
	{
		ImageValidationResults ValidateDiscCoverImage(ImageInfo imageInfo);

		IEnumerable<string> GetValidationResultsHints(ImageValidationResults results);

		bool IsSupportedFileFormat(string fileName);
	}
}
