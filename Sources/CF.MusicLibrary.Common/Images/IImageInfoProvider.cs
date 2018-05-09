using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.Common.Images
{
	public interface IImageInfoProvider
	{
		ImageInfo GetImageInfo(string imageFileName);
	}
}
