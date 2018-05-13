using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker
{
	public class LibraryInconsistencyFilter : ILibraryInconsistencyFilter
	{
		private readonly Lazy<List<AllowedArtistCorrection>> allowedArtistCorrections;
		private readonly Lazy<List<AllowedSongCorrection>> allowedSongsCorrections;

		private readonly ILogger<LibraryInconsistencyFilter> logger;
		private readonly CheckingSettings settings;

		public LibraryInconsistencyFilter(ILogger<LibraryInconsistencyFilter> logger, IOptions<CheckingSettings> options)
		{
			allowedArtistCorrections = new Lazy<List<AllowedArtistCorrection>>(LoadAllowedArtistCorrection);
			allowedSongsCorrections = new Lazy<List<AllowedSongCorrection>>(LoadAllowedSongsCorrection);
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public bool SkipInconsistency_DifferentGenresForDisc(Disc disc, IEnumerable<Genre> genres)
		{
			var uriString = disc.Uri.ToString();
			return uriString == "/Сборники/Best/Foreign" || uriString == "/Сборники/Best/Russian";
		}

		public bool SkipInconsistency_ArtistNameCorrected(string originalArtistName, string correctedArtistName)
		{
			return allowedArtistCorrections.Value.Any(c => c.OriginalArtistName == originalArtistName && c.CorrectedArtistName == correctedArtistName);
		}

		public bool SkipInconsistency_SongTitleCorrected(Song song, string correctedSongTitle)
		{
			string originalSongTitle = song.Title;

			if (String.Equals(originalSongTitle, correctedSongTitle, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			// Sometimes Last.fm replaces ’ with '
			correctedSongTitle = correctedSongTitle.Replace("’", "'");
			if (String.Equals(originalSongTitle, correctedSongTitle, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			// Sometimes Last.fm mistakenly replaces 'ё' with 'е'
			originalSongTitle = originalSongTitle.Replace("ё", "е");
			if (String.Equals(originalSongTitle, correctedSongTitle, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return allowedSongsCorrections.Value.Any(c =>
						c.Artist == song.Artist.Name && c.OriginalSongTitle == song.Title && c.CorrectedSongTitle == correctedSongTitle);
		}

		private List<AllowedArtistCorrection> LoadAllowedArtistCorrection()
		{
			var allowedArtistCorrectionsFileName = settings.AllowedArtistCorrectionsFileName;
			logger.LogInformation(Current($"Loading artist corrections from '${allowedArtistCorrectionsFileName}'..."));

			var allowedCorrections = new List<AllowedArtistCorrection>();
			Regex allowedArtistCorrectionRegex = new Regex("^(.+?)\\t+(.+?)$", RegexOptions.Compiled);
			foreach (var line in File.ReadLines(allowedArtistCorrectionsFileName))
			{
				if (line.Length == 0)
				{
					continue;
				}

				var match = allowedArtistCorrectionRegex.Match(line);
				if (!match.Success)
				{
					throw new InvalidInputDataException(Current($"Invalid artist correction line: '{line}'"));
				}

				allowedCorrections.Add(new AllowedArtistCorrection(match.Groups[1].Value, match.Groups[2].Value));
			}

			logger.LogInformation(Current($"Loaded {allowedCorrections.Count} artist name corrections"));

			return allowedCorrections;
		}

		private List<AllowedSongCorrection> LoadAllowedSongsCorrection()
		{
			var allowedSongsCorrectionsFileName = settings.AllowedSongCorrectionsFileName;
			logger.LogInformation(Current($"Loading songs corrections from '${allowedSongsCorrectionsFileName}'..."));

			var allowedCorrections = new List<AllowedSongCorrection>();
			Regex allowedArtistCorrectionRegex = new Regex("^(.+?)\\t+(.+?)\\t+(.+?)$", RegexOptions.Compiled);
			foreach (var line in File.ReadLines(allowedSongsCorrectionsFileName))
			{
				if (line.Length == 0)
				{
					continue;
				}

				var match = allowedArtistCorrectionRegex.Match(line);
				if (!match.Success)
				{
					throw new InvalidInputDataException(Current($"Invalid song correction line: '{line}'"));
				}

				allowedCorrections.Add(new AllowedSongCorrection(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
			}

			logger.LogInformation(Current($"Loaded {allowedCorrections.Count} songs corrections"));

			return allowedCorrections;
		}
	}
}
