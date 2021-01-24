using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MusicLibrary.Core.Facades;
using MusicLibrary.Services.Interfaces;

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

		public WorkshopMusicStorage(IDiscTitleToAlbumMapper discTitleToAlbumMapper, IFileSystemFacade fileSystemFacade, IOptions<DiscAdderSettings> options)
		{
			this.discTitleToAlbumMapper = discTitleToAlbumMapper ?? throw new ArgumentNullException(nameof(discTitleToAlbumMapper));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
			this.workshopRootPath = options?.Value?.WorkshopStoragePath ?? throw new ArgumentNullException(nameof(options));
		}

		public AddedDiscInfo GetAddedDiscInfo(string sourceDiscPath, IEnumerable<string> songFiles)
		{
			var discTreeTitle = Path.GetFileName(sourceDiscPath);
			ParseDiscData(discTreeTitle, out var year, out var title);

			var songs = ParseSongsData(songFiles).ToList();

			var destinationFolderPath = GetDestinationFolderPathForDisc(sourceDiscPath);

			return new AddedDiscInfo(songs)
			{
				Year = year,
				DiscTitle = title,
				TreeTitle = discTreeTitle,
				AlbumTitle = discTitleToAlbumMapper.GetAlbumTitleFromDiscTitle(title),

				// If all songs have artist parsed from song file name, then we leave disc artist empty.
				// If some songs have no artist parsed from song file name, then we take parent folder name as artist name.
				Artist = songs.All(song => song.Artist != null) ? null : destinationFolderPath.Last(),
				SourcePath = sourceDiscPath,
				DestinationFolderPath = destinationFolderPath,
			};
		}

		private static IEnumerable<AddedSongInfo> ParseSongsData(IEnumerable<string> songFiles)
		{
			var songs = songFiles.Select(GetSongInfo).ToList();

			// Should we keep Artist parsed from songs or should we clear it?
			// Currently artist is parsed from the song filename by the following regex: (.+) - (.+)
			// It works for file names like 'Aerosmith - I Don't Want To Miss A Thing.mp3',
			// but doesn't work for files like '09 - Lappi - I. Eramaajarvi.mp3'.
			// Here we determine whether major part of disc songs has artist in title.
			// If not, then we clear Artist in all songs from this disc.
			if (songs.Count(s => String.IsNullOrEmpty(s.Artist)) > songs.Count(s => !String.IsNullOrEmpty(s.Artist)))
			{
				foreach (var song in songs)
				{
					song.DismissArtistInfo();
				}
			}

			return songs;
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

			throw new InvalidOperationException($"Could not parse song data from '{songPath}'");
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
	}
}
