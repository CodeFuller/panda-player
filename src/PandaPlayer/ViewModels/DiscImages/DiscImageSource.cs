using System;

namespace PandaPlayer.ViewModels.DiscImages
{
	public class DiscImageSource
	{
		public bool IsImageForDeletedDisc { get; private init; }

		public bool IsMissingImage { get; private init; }

		public string FilePath { get; private init; }

		public static DiscImageSource ForDeletedDisc { get; } = new()
		{
			IsImageForDeletedDisc = true,
		};

		public static DiscImageSource ForMissingImage { get; } = new()
		{
			IsMissingImage = true,
		};

		public static DiscImageSource ForImage(Uri imageContentUri)
		{
			if (!imageContentUri.IsFile)
			{
				throw new ArgumentException($"Image content URI does not point to the file: '{imageContentUri}'");
			}

			return new DiscImageSource
			{
				FilePath = imageContentUri.OriginalString,
			};
		}

		private DiscImageSource()
		{
		}
	}
}
