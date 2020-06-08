using MusicLibrary.Core.Objects;
using MusicLibrary.Core.Objects.Images;

namespace MusicLibrary.DiscPreprocessor.AddingToLibrary
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
