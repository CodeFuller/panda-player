using System;
using CF.Library.Core.Extensions;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL
{
	public class StorageUrlBuilder : IStorageUrlBuilder
	{
		public Uri BuildArtistStorageUrl(params string[] segments)
		{
			return AppendLeadingSlash(BuildUriFromSegments(segments));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
		public Uri BuildAlbumStorageUrl(Uri artistUri, string albumName)
		{
			return artistUri.Combine(albumName);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
		public Uri BuildSongStorageUrl(Uri albumUri, string songName)
		{
			return albumUri.Combine(songName);
		}

		public Uri ReplaceArtistName(Uri artistStorageUri, string newArtistName)
		{
			return ReplaceLastSegment(artistStorageUri, newArtistName);
		}

		public Uri ReplaceAlbumName(Uri albumStorageUri, string newAlbumName)
		{
			return ReplaceLastSegment(albumStorageUri, newAlbumName);
		}

		private static Uri ReplaceLastSegment(Uri sourceUri, string newSegment)
		{
			if (sourceUri == null)
			{
				throw new ArgumentNullException(nameof(sourceUri));
			}
			if (newSegment == null)
			{
				throw new ArgumentNullException(nameof(newSegment));
			}

			if (sourceUri.ToString().EndsWith(UriExtensions.UriSeparatorString, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(Current($"Incorrect storage URI: {sourceUri}. URI should not end with '{UriExtensions.UriSeparatorString}'"));
			}

			var segments = sourceUri.SegmentsEx();
			if (segments.Length == 0)
			{
				throw new ArgumentException(Current($"Incorrect storage URI: {sourceUri}"));
			}
			segments[segments.Length - 1] = newSegment;
			return BuildUriFromSegments(segments);
		}

		public Uri MapWorkshopStoragePath(string pathWithinStorage)
		{
			if (pathWithinStorage == null)
			{
				throw new ArgumentNullException(nameof(pathWithinStorage));
			}

			return AppendLeadingSlash(BuildUriFromSegments(pathWithinStorage.Split('\\')));
		}

		private static Uri BuildUriFromSegments(params string[] segments)
		{
			return new Uri(String.Join(UriExtensions.UriSeparatorString, segments), UriKind.Relative);
		}

		private static Uri AppendLeadingSlash(Uri uri)
		{
			string uriString = uri.ToString();
			if (!uriString.StartsWith(UriExtensions.UriSeparatorString, StringComparison.OrdinalIgnoreCase))
			{
				uriString = UriExtensions.UriSeparatorString + uriString;
				return new Uri(uriString, uri.GetUriKind());
			}
			else
			{
				return uri;
			}
		}
	}
}
