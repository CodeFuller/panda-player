using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Configuration;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Application;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.LibraryChecker
{
	internal struct AllowedArtistCorrection
	{
		public string OriginalArtistName { get; }

		public string CorrectedArtistName { get; }

		public AllowedArtistCorrection(string originalArtistName, string correctedArtistName)
		{
			OriginalArtistName = originalArtistName;
			CorrectedArtistName = correctedArtistName;
		}
	}

	internal struct AllowedSongCorrection
	{
		public string Artist { get; }

		public string OriginalSongTitle { get; }

		public string CorrectedSongTitle { get; }

		public AllowedSongCorrection(string artist, string originalSongTitle, string correctedSongTitle)
		{
			Artist = artist;
			OriginalSongTitle = originalSongTitle;
			CorrectedSongTitle = correctedSongTitle;
		}
	}

	public class LibraryInconsistencyFilter : ILibraryInconsistencyFilter
	{
		private readonly Lazy<List<AllowedArtistCorrection>> allowedArtistCorrections;
		private readonly Lazy<List<AllowedSongCorrection>> allowedSongsCorrections;

		public LibraryInconsistencyFilter()
		{
			allowedArtistCorrections = new Lazy<List<AllowedArtistCorrection>>(LoadAllowedArtistCorrection);
			allowedSongsCorrections = new Lazy<List<AllowedSongCorrection>>(LoadAllowedSongsCorrection);
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

			//	Sometimes Last.fm replaces ’ with '
			correctedSongTitle = correctedSongTitle.Replace("’", "'");
			if (String.Equals(originalSongTitle, correctedSongTitle, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			//	Sometimes Last.fm mistakenly replaces 'ё' with 'е'
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
			var allowedArtistCorrectionsFileName = Path.Combine(AppSettings.GetRequiredValue<string>("AppDataPath"), "AllowedLastFmArtistCorrections.txt");
			Logger.WriteInfo(Current($"Loading artist corrections from '${allowedArtistCorrectionsFileName}'..."));

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

			Logger.WriteInfo(Current($"Loaded {allowedCorrections.Count} artist name corrections"));

			return allowedCorrections;
		}

		private List<AllowedSongCorrection> LoadAllowedSongsCorrection()
		{
			var allowedSongsCorrectionsFileName = Path.Combine(AppSettings.GetRequiredValue<string>("AppDataPath"), "AllowedSongCorrections.txt");
			Logger.WriteInfo(Current($"Loading songs corrections from '${allowedSongsCorrectionsFileName}'..."));

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

			Logger.WriteInfo(Current($"Loaded {allowedCorrections.Count} songs corrections"));

			return allowedCorrections;
		}
	}
}
