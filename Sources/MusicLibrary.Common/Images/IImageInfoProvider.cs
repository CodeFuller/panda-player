using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.Common.Images
{
	public interface IImageInfoProvider
	{
		ImageInfo GetImageInfo(string imageFileName);
	}
}
