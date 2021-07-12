using PandaPlayer.Core.Models;
using PandaPlayer.Shared.Images;

namespace PandaPlayer.DiscAdder.AddedContent
{
	internal class AddedDiscImage
	{
		public DiscModel Disc { get; }

		public ImageInfo ImageInfo { get; }

		public AddedDiscImage(DiscModel disc, ImageInfo imageInfo)
		{
			Disc = disc;
			ImageInfo = imageInfo;
		}
	}
}
