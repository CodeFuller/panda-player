using System;
using System.Linq;
using MusicLibrary.Core;

namespace MusicLibrary.LibraryToolkit.Extensions
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
