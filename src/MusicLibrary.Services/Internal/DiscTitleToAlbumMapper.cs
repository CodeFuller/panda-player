using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Settings;

namespace MusicLibrary.Services.Internal
{
	internal class DiscTitleToAlbumMapper : IDiscTitleToAlbumMapper
	{
		private readonly List<Regex> discToAlbumPatterns = new List<Regex>();
		private readonly List<Regex> emptyAlbumTitlePatterns = new List<Regex>();

		public DiscTitleToAlbumMapper(IOptions<DiscToAlbumMappingSettings> options)
		{
			var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			foreach (var pattern in settings.AlbumTitlePatterns ?? Enumerable.Empty<string>())
			{
				discToAlbumPatterns.Add(new Regex(pattern, RegexOptions.Compiled));
			}

			foreach (var pattern in settings.EmptyAlbumTitlePatterns ?? Enumerable.Empty<string>())
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
	}
}
