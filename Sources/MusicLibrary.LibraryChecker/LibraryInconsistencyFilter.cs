using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker
{
	public class LibraryInconsistencyFilter : ILibraryInconsistencyFilter
	{
		private readonly CheckingSettings settings;
		private readonly InconsistencyFilterSettings filteringSettings;
		private readonly List<Regex> skippedDifferentGenresDiscUris = new List<Regex>();

		public LibraryInconsistencyFilter(IOptions<CheckingSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
			this.filteringSettings = settings.InconsistencyFilter;
			this.skippedDifferentGenresDiscUris.AddRange(filteringSettings.SkipDifferentGenresForDiscs.Select(r => new Regex(r, RegexOptions.Compiled)));
		}

		public bool ShouldSkipDifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			var uriString = disc.Uri.ToString();
			return skippedDifferentGenresDiscUris.Any(re => re.IsMatch(uriString));
		}

		public bool ShouldSkipArtistNameCorrection(string originalArtistName, string correctedArtistName)
		{
			foreach (var correction in filteringSettings.AllowedLastFmArtistCorrections)
			{
				var correctedOriginalArtistName = correction.CorrectArtistName(originalArtistName);
				if (String.Equals(correctedOriginalArtistName, correctedArtistName, StringComparison.Ordinal))
				{
					return true;
				}
			}

			return false;
		}

		public bool ShouldSkipSongTitleCorrection(Song song, string correctedSongTitle)
		{
			string originalSongTitle = song.Title;

			// We ignore case here. Without this, there will be too much song title inconsistencies.
			if (String.Equals(originalSongTitle, correctedSongTitle, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			foreach (var correction in filteringSettings.LastFmSongTitleCharacterCorrections)
			{
				var correctedOriginalTitle = originalSongTitle.Replace(correction.Original, correction.Corrected);
				if (String.Equals(correctedOriginalTitle, correctedSongTitle, StringComparison.Ordinal))
				{
					return true;
				}
			}

			return filteringSettings.AllowedLastFmSongCorrections.Any(c =>
						String.Equals(c.Artist, song.Artist.Name, StringComparison.Ordinal) &&
						String.Equals(c.Original, song.Title, StringComparison.Ordinal) &&
						String.Equals(c.Corrected, correctedSongTitle, StringComparison.Ordinal));
		}
	}
}
