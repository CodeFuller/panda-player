using System;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.Local
{
	internal class LocalLibraryDiscPath
	{
		private readonly ItemUriParts pathParts;

		public LocalLibraryDiscPath(Disc disc) :
			this(disc.Uri)
		{
		}

		public LocalLibraryDiscPath(Uri discUri)
		{
			pathParts = new ItemUriParts(discUri);

			if (pathParts.Count < 2)
			{
				throw new InvalidOperationException(Current($"Could not extract category and nested directory from disc path {discUri.LocalPath}"));
			}
		}

		public string Category => pathParts[0];

		public string NestedDirectory => pathParts[1];
	}
}
