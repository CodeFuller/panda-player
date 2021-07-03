using System;

namespace MusicLibrary.Services.IntegrationTests.Extensions
{
	internal static class StringExtensions
	{
		public static Uri ToContentUri(this string relativePath, string libraryStorageRoot)
		{
			var relativeUri = new Uri(relativePath, UriKind.Relative);
			return new Uri(new Uri(libraryStorageRoot), relativeUri);
		}
	}
}
