using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	internal class DiscPathParts
	{
		private readonly List<string> pathParts;

		public DiscPathParts(Disc disc, string libraryRootDirectory)
		{
			string albumFullPath = disc.Uri.LocalPath;
			if (!albumFullPath.StartsWith(libraryRootDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Current($"Album path '{albumFullPath}' is not within library root directory {libraryRootDirectory}"));
			}
			string albumSubPath = albumFullPath.Substring(libraryRootDirectory.Length);

			pathParts = albumSubPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToList();
			if (pathParts.Count < 2)
			{
				throw new InvalidOperationException(Current($"Could not extract category and nested directory from disc path {albumFullPath}"));
			}
		}

		public string Category => pathParts[0];

		public string NestedDirectory => pathParts[1];
	}
}
