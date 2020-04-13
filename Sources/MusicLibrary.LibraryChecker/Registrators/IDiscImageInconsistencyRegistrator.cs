using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.LibraryChecker.Registrators
{
	public interface IDiscImageInconsistencyRegistrator
	{
		void RegisterDiscCoverIsTooSmall(Disc disc, ImageInfo imageInfo);

		void RegisterDiscCoverIsTooBig(Disc disc, ImageInfo imageInfo);

		void RegisterImageFileIsTooBig(Disc disc, ImageInfo imageInfo);

		void RegisterImageHasUnsupportedFormat(Disc disc, ImageInfo imageInfo);
	}
}
