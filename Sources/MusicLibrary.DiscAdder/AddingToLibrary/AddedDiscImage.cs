using MusicLibrary.Core.Models;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.DiscAdder.AddingToLibrary
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
