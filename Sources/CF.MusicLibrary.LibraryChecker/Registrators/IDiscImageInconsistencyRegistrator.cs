using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscImageInconsistencyRegistrator
	{
		void RegisterDiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo);

		void RegisterDiscCoverIsTooBig(Disc disc, ImageInfo imageInfo);

		void RegisterImageFileIsTooBig(Disc disc, ImageInfo imageInfo);

		void RegisterImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo);
	}
}
