using System;
using System.Linq;
using CF.MusicLibrary.Core;

namespace CF.MusicLibrary.LibraryToolkit.Extensions
{
	public static class UriExtensions
	{
		public static string GetLastPart(this Uri uri)
		{
			var uriParts = new ItemUriParts(uri);
			return uriParts.Last();
		}
	}
}
