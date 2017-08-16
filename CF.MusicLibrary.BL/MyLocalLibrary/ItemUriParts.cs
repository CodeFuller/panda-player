using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;
using static System.FormattableString;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	public class ItemUriParts : IEnumerable<string>
	{
		private const string UriDelimiter = "/";

		private readonly string[] parts;

		public static Uri RootUri => new Uri(UriDelimiter, UriKind.Relative);

		public int Count => parts.Length;

		public string this[int i] => parts[i];

		public string PathWithinLibrary => Path.Combine(parts.ToArray());

		public ItemUriParts(string path, string libraryRootDirectory)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			if (libraryRootDirectory == null)
			{
				throw new ArgumentNullException(nameof(libraryRootDirectory));
			}

			libraryRootDirectory = libraryRootDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			libraryRootDirectory += Path.DirectorySeparatorChar;

			if (!path.StartsWith(libraryRootDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException(Current($"Path '{path}' is not within library root directory {libraryRootDirectory}"));
			}
			string albumSubPath = path.Substring(libraryRootDirectory.Length);

			parts = albumSubPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}

		public ItemUriParts(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			if (uri.IsAbsoluteUri || !uri.ToString().StartsWith(UriDelimiter, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(Current($"Invalid item URI: '{uri}'. Item URI should be relative and should start from '{UriDelimiter}'."));
			}

			parts = uri.SegmentsEx().Skip(1).ToArray();
		}

		public static Uri Join(IEnumerable<string> parts)
		{
			var partsList = parts.ToList();
			if (partsList.Count == 0)
			{
				return RootUri;
			}

			return new Uri(partsList.Aggregate(String.Empty, (current, part) => Invariant($"{current}{UriDelimiter}{part}")), UriKind.Relative);
		}

		public bool IsBaseOf(ItemUriParts uriParts)
		{
			if (uriParts == null)
			{
				throw new ArgumentNullException(nameof(uriParts));
			}

			if (Count >= uriParts.Count)
			{
				return false;
			}

			return parts.SequenceEqual(uriParts.parts.Take(Count));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return parts.Select(s => s).GetEnumerator();
		}
	}
}
