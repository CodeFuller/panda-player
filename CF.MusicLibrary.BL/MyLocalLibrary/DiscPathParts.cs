using System;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	public class DiscPathParts
	{
		private readonly string[] parts;

		public int Count => parts.Length;

		public string this[int i] => parts[i];

		public string PathWithinLibrary => Path.Combine(parts.ToArray());

		public DiscPathParts(string discPath, string libraryRootDirectory)
		{
			if (discPath == null)
			{
				throw new ArgumentNullException(nameof(discPath));
			}
			if (libraryRootDirectory == null)
			{
				throw new ArgumentNullException(nameof(libraryRootDirectory));
			}

			libraryRootDirectory = libraryRootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			libraryRootDirectory += Path.DirectorySeparatorChar;

			if (!discPath.StartsWith(libraryRootDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Current($"Disc path '{discPath}' is not within library root directory {libraryRootDirectory}"));
			}
			string albumSubPath = discPath.Substring(libraryRootDirectory.Length);

			parts = albumSubPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}

		public DiscPathParts(Uri discUri)
		{
			if (discUri == null)
			{
				throw new ArgumentNullException(nameof(discUri));
			}

			if (discUri.IsAbsoluteUri || !discUri.ToString().StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(Current($"Invalid disc URI: '{discUri}'. Disc URI should be relative and should start from '/'."));
			}
			
			parts = discUri.SegmentsEx().Skip(1).ToArray();
		}
	}
}
