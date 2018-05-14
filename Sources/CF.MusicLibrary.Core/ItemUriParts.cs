using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.Core
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Following ViewModel naming convention")]
	public class ItemUriParts : IEnumerable<string>
	{
		private const string UriDelimiter = "/";

		private readonly string[] parts;

		public static Uri RootUri => new Uri(UriDelimiter, UriKind.Relative);

		public Uri Uri => Join(parts);

		public int Count => parts.Length;

		public string this[int i] => parts[i];

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
				throw new InvalidOperationException(FormattableStringExtensions.Current($"Path '{path}' is not within library root directory {libraryRootDirectory}"));
			}

			string subPath = path.Substring(libraryRootDirectory.Length);
			parts = subPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}

		public ItemUriParts(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			if (uri.IsAbsoluteUri || !uri.ToString().StartsWith(UriDelimiter, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(FormattableStringExtensions.Current($"Invalid item URI: '{uri}'. Item URI should be relative and should start from '{UriDelimiter}'."));
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

			return new Uri(partsList.Aggregate(String.Empty, (current, part) => FormattableString.Invariant($"{current}{UriDelimiter}{part}")), UriKind.Relative);
		}

		public static Uri Join(Uri baseUri, string addedPart)
		{
			return new Uri(FormattableString.Invariant($"{baseUri}{UriDelimiter}{addedPart}"), UriKind.Relative);
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
