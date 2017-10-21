using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Core.Objects.Images;

namespace CF.MusicLibrary.DiscPreprocessor.AddingToLibrary
{
	public class AddedDiscImage
	{
		public Disc Disc { get; }

		public ImageInfo ImageInfo { get; }

		public AddedDiscImage(Disc disc, ImageInfo imageInfo)
		{
			Disc = disc;
			ImageInfo = imageInfo;
		}
	}
}
