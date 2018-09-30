using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core;
using CF.MusicLibrary.Core.Interfaces;
using Microsoft.Extensions.Options;
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

		private readonly IDiscTitleToAlbumMapper discTitleToAlbumMapper;
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly string workshopRootPath;

		public WorkshopMusicStorage(IDiscTitleToAlbumMapper discTitleToAlbumMapper, IFileSystemFacade fileSystemFacade, IOptions<DiscPreprocessorSettings> options)
		{
			this.discTitleToAlbumMapper = discTitleToAlbumMapper ?? throw new ArgumentNullException(nameof(discTitleToAlbumMapper));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.workshopRootPath = options?.Value?.WorkshopStoragePath ?? throw new ArgumentNullException(nameof(options));
		}

		public AddedDiscInfo GetAddedDiscInfo(string discPath, IEnumerable<string> songFiles)
		{
			ItemUriParts uriParts = new ItemUriParts(discPath, workshopRootPath);
			if (uriParts.Count == 0)
			{
				throw new InvalidInputDataException(Current($"Could not parse disc data from '{uriParts[1]}'"));
			}

			ParseDiscData(uriParts[uriParts.Count - 1], out var year, out var title);

			List<AddedSongInfo> songs = songFiles.Select(GetSongInfo).ToList();

			AddedDiscInfo discInfo = new AddedDiscInfo(songs)
			{
				Year = year,
				DiscTitle = title,
				AlbumTitle = discTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(title),
				SourcePath = discPath,
				UriWithinStorage = uriParts.Uri,
			};

			if (IsArtistCategory(uriParts[0]) && uriParts.Count > 2)
			{
				discInfo.DiscType = DsicType.ArtistDisc;
				discInfo.Artist = uriParts[uriParts.Count - 2];
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

		public void DeleteSourceContent(IEnumerable<string> contentFiles)
		{
			foreach (var file in contentFiles)
			{
				fileSystemFacade.ClearReadOnlyAttribute(file);
				fileSystemFacade.DeleteFile(file);
			}

			foreach (var subDirectory in fileSystemFacade.EnumerateDirectories(workshopRootPath))
			{
				List<string> files = new List<string>();
				FindDirectoryFiles(subDirectory, files);

				if (files.Any())
				{
					return;
				}

				fileSystemFacade.DeleteDirectory(subDirectory, true);
			}
		}

		private void FindDirectoryFiles(string directoryPath, List<string> files)
		{
			foreach (string subDirectory in fileSystemFacade.EnumerateDirectories(directoryPath))
			{
				FindDirectoryFiles(subDirectory, files);
			}

			foreach (string file in fileSystemFacade.EnumerateFiles(directoryPath))
			{
				files.Add(file);
			}
		}

		private static bool IsArtistCategory(string category)
		{
			var artistCategories = new[]
			{
				"Belarussian",
				"Foreign",
				"Russian",
			};

			return artistCategories.Any(d => String.Equals(d, category, StringComparison.OrdinalIgnoreCase));
		}
	}
}
