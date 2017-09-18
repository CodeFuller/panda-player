using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.BL;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.MusicStorage
{
	/// <summary>
	/// Class for music storage that keeps discs for adding to library.
	/// </summary>
	internal class WorkshopMusicStorage : IWorkshopMusicStorage
	{
		private static readonly Regex DiscDataRegex = new Regex(@"^(\d{4}) - (.+)$", RegexOptions.Compiled);
		private static readonly Regex SongWithTrackAndArtistRegex = new Regex(@"^(\d{2}) - ((.+) - (.+))$", RegexOptions.Compiled);
		private static readonly Regex SongWithTrackRegex = new Regex(@"^(\d{2}) - (.+)$", RegexOptions.Compiled);
		private static readonly Regex SongWithArtistRegex = new Regex(@"^((.+) - (.+))$", RegexOptions.Compiled);

		private readonly string workshopRootPath;

		public WorkshopMusicStorage(string workshopRootPath)
		{
			this.workshopRootPath = workshopRootPath;
		}

		public AddedDiscInfo GetAddedDiscInfo(string discPath, IEnumerable<string> songFiles)
		{
			List<AddedSongInfo> songs = songFiles.Select(GetSongInfo).ToList();

			ItemUriParts pathParts = new ItemUriParts(discPath, workshopRootPath);
			if (pathParts.Count == 0)
			{
				throw new InvalidInputDataException(Current($"Could not parse disc data from '{pathParts[1]}'"));
			}

			short? year;
			string title;
			ParseDiscData(pathParts[pathParts.Count - 1], out year, out title);

			AddedDiscInfo discInfo = new AddedDiscInfo(songs)
			{
				Year = year,
				Title = title,
				SourcePath = discPath,
				PathWithinStorage = pathParts.PathWithinLibrary,
				NameInStorage = pathParts[pathParts.Count - 1],
			};

			//	Case of "Artist \ Disc", e.g. "Nightwish\2011 - Imaginaerum"
			if (pathParts.Count == 2 && year.HasValue)
			{
				discInfo.DiscType = DsicType.ArtistDisc;
				discInfo.Artist = pathParts[0];
			}
			else
			{
				bool hasSongWithoutArtist = songs.Any(s => String.IsNullOrEmpty(s.Artist));
				bool hasSongWithArtist = songs.Any(s => !String.IsNullOrEmpty(s.Artist));
				if (hasSongWithoutArtist && hasSongWithArtist)
				{
					throw new InvalidInputDataException(Current($"Disc '{discPath}' has songs with and without artist name. This mode currently is not supported"));
				}

				discInfo.DiscType = hasSongWithArtist
					? DsicType.CompilationDiscWithArtistInfo
					: DsicType.CompilationDiscWithoutArtistInfo;
			}

			return discInfo;
		}

		private static void ParseDiscData(string discName, out short? year, out string title)
		{
			var match = DiscDataRegex.Match(discName);
			if (match.Success)
			{
				year = Int16.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				title = match.Groups[2].Value;
			}
			else
			{
				year = null;
				title = discName;
			}
		}

		private AddedSongInfo GetSongInfo(string songPath)
		{
			string name = Path.GetFileNameWithoutExtension(songPath);

			var match = SongWithTrackAndArtistRegex.Match(name);
			if (match.Success)
			{
				return new AddedSongInfo(songPath)
				{
					Track = Int16.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
					Artist = match.Groups[3].Value,
					Title = match.Groups[4].Value,
					FullTitle = match.Groups[2].Value,
				};
			}

			match = SongWithTrackRegex.Match(name);
			if (match.Success)
			{
				return new AddedSongInfo(songPath)
				{
					Track = Int16.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
					Artist = null,
					Title = match.Groups[2].Value,
					FullTitle = match.Groups[2].Value,
				};
			}

			match = SongWithArtistRegex.Match(name);
			if (match.Success)
			{
				return new AddedSongInfo(songPath)
				{
					Track = null,
					Artist = match.Groups[2].Value,
					Title = match.Groups[3].Value,
					FullTitle = match.Groups[1].Value,
				};
			}

			throw new InvalidInputDataException(Current($"Could not parse song data from '{songPath}'"));
		}
	}
}
