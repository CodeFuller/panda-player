using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CF.MusicLibrary.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.Core
{
	public class DiscTitleToAlbumMapper : IDiscTitleToAlbumMapper
	{
		private readonly List<Regex> discToAlbumPatterns = new List<Regex>();
		private readonly List<Regex> emptyAlbumTitlePatterns = new List<Regex>();

		public DiscTitleToAlbumMapper(IOptions<DiscToAlbumMappingSettings> options)
		{
			var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			foreach (var pattern in settings.AlbumTitlePatterns)
			{
				discToAlbumPatterns.Add(new Regex(pattern, RegexOptions.Compiled));
			}

			foreach (var pattern in settings.EmptyAlbumTitlePatterns)
			{
				emptyAlbumTitlePatterns.Add(new Regex(pattern, RegexOptions.Compiled));
			}
		}

		public string GetAlbumTitleFromDiscTitle(string discTitle)
		{
			if (discTitle == null)
			{
				return null;
			}

			var albumTitle = discTitle;
			foreach (var pattern in discToAlbumPatterns)
			{
				var match = pattern.Match(albumTitle);
				if (match.Success)
				{
					albumTitle = match.Groups[1].Value;
				}
			}

			foreach (var pattern in emptyAlbumTitlePatterns)
			{
				if (pattern.IsMatch(albumTitle))
				{
					return null;
				}
			}

			return albumTitle;
		}

		public bool AlbumTitleIsSuspicious(string albumTitle)
		{
			return !String.Equals(GetAlbumTitleFromDiscTitle(albumTitle), albumTitle, StringComparison.Ordinal);
		}
	}
}
