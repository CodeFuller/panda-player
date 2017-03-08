using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class AlbumCrawler
	{
		private readonly ISongFileFilter songFileFilter;

		public AlbumCrawler(ISongFileFilter songFileFilter)
		{
			if (songFileFilter == null)
			{
				throw new ArgumentNullException(nameof(songFileFilter));
			}

			this.songFileFilter = songFileFilter;
		}

		public IEnumerable<AlbumContent> LoadAlbums(string albumsDirectory)
		{
			return LoadAlbums(new DirectoryInfo(albumsDirectory));
		}

		private IEnumerable<AlbumContent> LoadAlbums(DirectoryInfo directoryInfo)
		{
			List<string> songFiles = directoryInfo.GetFiles().
				Where(IsSongFile).
				OrderBy(f => f.Name).
				Select(f => f.FullName).
				ToList();

			List<AlbumContent> nestedAlbums = new List<AlbumContent>();
			foreach (var subDirectory in directoryInfo.GetDirectories().OrderBy(x => x.Name))
			{
				nestedAlbums.AddRange(LoadAlbums(subDirectory));

				if (songFiles.Any() && nestedAlbums.Any())
				{
					throw new InvalidOperationException($"Directory '{directoryInfo.FullName}' has both songs and albums. It's and invalid album structure.");
				}
			}

			if (songFiles.Any())
			{
				return Enumerable.Repeat(new AlbumContent(directoryInfo.FullName, songFiles.Where(songFileFilter.IsSongFile).Select(Path.GetFileName)), 1);
			}
			else
			{
				return nestedAlbums;
			}
		}

		private bool IsSongFile(FileInfo fileInfo)
		{
			return true;
		}
	}
}
