using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CF.Library.Core.Exceptions;
using CF.MusicLibrary.BL.MyLocalLibrary;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.AlbumPreprocessor.MusicStorage
{
	/// <summary>
	/// Music storage that keeps albums for adding to main music library.
	/// </summary>
	internal class LocalWorkshopMusicStorage : IWorkshopMusicStorage
	{
		private static readonly Regex AlbumDataRegex = new Regex(@"^(\d{4}) - (.+)$", RegexOptions.Compiled);
		private static readonly Regex SongWithTrackAndArtistRegex = new Regex(@"^(\d{2}) - ((.+) - (.+))$", RegexOptions.Compiled);
		private static readonly Regex SongWithTrackRegex = new Regex(@"^(\d{2}) - (.+)$", RegexOptions.Compiled);
		private static readonly Regex SongWithArtistRegex = new Regex(@"^((.+) - (.+))$", RegexOptions.Compiled);

		private readonly string workshopRootPath;

		public LocalWorkshopMusicStorage(string workshopRootPath)
		{
			this.workshopRootPath = workshopRootPath;
		}

		public AlbumInfo GetAlbumInfo(string albumPath, IEnumerable<string> songFiles)
		{
			List<SongInfo> songs = songFiles.Select(GetSongInfo).ToList();

			ItemUriParts pathParts = new ItemUriParts(albumPath, workshopRootPath);
			if (pathParts.Count == 0)
			{
				throw new InvalidInputDataException(Current($"Could not parse album data from '{pathParts[1]}'"));
			}

			int? year;
			string title;
			ParseAlbumData(pathParts[pathParts.Count - 1], out year, out title);

			AlbumInfo albumInfo = new AlbumInfo(songs)
			{
				Year = year,
				Title = title,
			};

			//	Case of "Artist \ Album", e.g. "Nightwish\2011 - Imaginaerum"
			if (pathParts.Count == 2 && year.HasValue)
			{
				albumInfo.AlbumType = AlbumType.ArtistAlbum;
				albumInfo.Artist = pathParts[0];
			}
			else
			{
				bool hasSongWithoutArtist = songs.Any(s => String.IsNullOrEmpty(s.Artist));
				bool hasSongWithArtist = songs.Any(s => !String.IsNullOrEmpty(s.Artist));
				if (hasSongWithoutArtist && hasSongWithArtist)
				{
					throw new InvalidInputDataException(Current($"Album '{albumPath}' has songs with and without artist name. This mode currently is not supported"));
				}

				albumInfo.AlbumType = hasSongWithArtist
					? AlbumType.CompilationAlbumWithArtistInfo
					: AlbumType.CompilationAlbumWithoutArtistInfo;
			}

			albumInfo.PathWithinStorage = pathParts.PathWithinLibrary;
			albumInfo.NameInStorage = pathParts[pathParts.Count - 1];

			return albumInfo;
		}

		private static void ParseAlbumData(string albumName, out int? year, out string title)
		{
			var match = AlbumDataRegex.Match(albumName);
			if (match.Success)
			{
				year = Int32.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				title = match.Groups[2].Value;
			}
			else
			{
				year = null;
				title = albumName;
			}
		}

		private SongInfo GetSongInfo(string songPath)
		{
			string name = Path.GetFileNameWithoutExtension(songPath);

			var match = SongWithTrackAndArtistRegex.Match(name);
			if (match.Success)
			{
				return new SongInfo(songPath)
				{
					Track = Int32.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
					Artist = match.Groups[3].Value,
					Title = match.Groups[4].Value,
					FullTitle = match.Groups[2].Value,
				};
			}

			match = SongWithTrackRegex.Match(name);
			if (match.Success)
			{
				return new SongInfo(songPath)
				{
					Track = Int32.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
					Artist = null,
					Title = match.Groups[2].Value,
					FullTitle = match.Groups[2].Value,
				};
			}

			match = SongWithArtistRegex.Match(name);
			if (match.Success)
			{
				return new SongInfo(songPath)
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
