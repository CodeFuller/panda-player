using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using Microsoft.Extensions.Options;
using MusicLibrary.Services.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.DiscAdder.MusicStorage
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

		public AddedDiscInfo GetAddedDiscInfo(string sourceDiscPath, IEnumerable<string> songFiles)
		{
			var discTreeTitle = Path.GetFileName(sourceDiscPath);

			ParseDiscData(discTreeTitle, out var year, out var title);

			var songs = songFiles.Select(GetSongInfo).ToList();

			var destinationFolderPath = GetDestinationFolderPathForDisc(sourceDiscPath);

			var discInfo = new AddedDiscInfo(songs)
			{
				Year = year,
				DiscTitle = title,
				TreeTitle = discTreeTitle,
				AlbumTitle = discTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(title),
				SourcePath = sourceDiscPath,
				DestinationFolderPath = destinationFolderPath,
			};

			if (IsArtistCategory(destinationFolderPath.First()) && destinationFolderPath.Count >= 2)
			{
				discInfo.DiscType = DsicType.ArtistDisc;
				discInfo.Artist = destinationFolderPath.Last();
			}
			else
			{
				var hasSongWithoutArtist = songs.Any(s => String.IsNullOrEmpty(s.Artist));
				var hasSongWithArtist = songs.Any(s => !String.IsNullOrEmpty(s.Artist));
				if (hasSongWithoutArtist && hasSongWithArtist)
				{
					throw new InvalidInputDataException(Current($"Disc '{sourceDiscPath}' has songs with and without artist name. This mode currently is not supported"));
				}

				discInfo.DiscType = hasSongWithArtist
					? DsicType.CompilationDiscWithArtistInfo
					: DsicType.CompilationDiscWithoutArtistInfo;
			}

			return discInfo;
		}

		private IReadOnlyCollection<string> GetDestinationFolderPathForDisc(string sourceDiscPath)
		{
			var sourceFolderPath = Path.GetDirectoryName(sourceDiscPath);
			var relativePath = Path.GetRelativePath(workshopRootPath, sourceFolderPath);
			return relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		}

		private static void ParseDiscData(string discTreeTitle, out short? year, out string title)
		{
			var match = DiscDataRegex.Match(discTreeTitle);
			if (match.Success)
			{
				year = Int16.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				title = match.Groups[2].Value;
			}
			else
			{
				year = null;
				title = discTreeTitle;
			}
		}

		private static AddedSongInfo GetSongInfo(string songPath)
		{
			var treeTitle = Path.GetFileName(songPath);
			var name = Path.GetFileNameWithoutExtension(songPath);

			var match = SongWithTrackAndArtistRegex.Match(name);
			if (match.Success)
			{
				return new AddedSongInfo(songPath)
				{
					Track = Int16.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
					Artist = match.Groups[3].Value,
					Title = match.Groups[4].Value,
					TreeTitle = treeTitle,
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
					TreeTitle = treeTitle,
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
					TreeTitle = treeTitle,
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
				var files = new List<string>();
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
			foreach (var subDirectory in fileSystemFacade.EnumerateDirectories(directoryPath))
			{
				FindDirectoryFiles(subDirectory, files);
			}

			foreach (var file in fileSystemFacade.EnumerateFiles(directoryPath))
			{
				files.Add(file);
			}
		}

		private static bool IsArtistCategory(string category)
		{
			var artistCategories = new[]
			{
				// TODO: Remove this hardcoded content details.
				"Belarussian",
				"Foreign",
				"Russian",
			};

			return artistCategories.Any(d => String.Equals(d, category, StringComparison.OrdinalIgnoreCase));
		}
	}
}
