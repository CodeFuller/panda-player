using System;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class AlbumContentComparer : IAlbumContentComparer
	{
		public void SetAlbumsCorrectness(AlbumTreeViewModel ethalonAlbums, AlbumTreeViewModel currentAlbums)
		{
			if (ethalonAlbums == null)
			{
				throw new ArgumentNullException(nameof(ethalonAlbums));
			}
			if (currentAlbums == null)
			{
				throw new ArgumentNullException(nameof(currentAlbums));
			}

			for (var i = 0; i < Math.Max(ethalonAlbums.Albums.Count, currentAlbums.Albums.Count); ++i)
			{
				var ethalonAlbum = i < ethalonAlbums.Albums.Count ? ethalonAlbums.Albums[i] : null;
				var currentAlbum = i < currentAlbums.Albums.Count ? currentAlbums.Albums[i] : null;
				SetAlbumsCorrectness(ethalonAlbum, currentAlbum);
			}
		}

		private static void SetAlbumsCorrectness(AlbumTreeViewItem ethalonAlbum, AlbumTreeViewItem currentAlbum)
		{
			if (ethalonAlbum == null && currentAlbum == null)
			{
				throw new InvalidOperationException();
			}

			if (ethalonAlbum == null)
			{
				MarkAlbumSongsAsIncorrect(currentAlbum);
			}
			else if (currentAlbum == null)
			{
				MarkAlbumSongsAsIncorrect(ethalonAlbum);
			}
			else
			{
				var ethalonSongs = ethalonAlbum.Songs.ToList();
				var currentSongs = currentAlbum.Songs.ToList();
				for (var i = 0; i < Math.Max(ethalonSongs.Count, currentSongs.Count); ++i)
				{
					var ethalonSong = i < ethalonSongs.Count ? ethalonSongs[i] : null;
					var currentSong = i < currentSongs.Count ? currentSongs[i] : null;
					SetSongsCorrectness(i + 1, ethalonSong, currentSong);
				}

				if (ethalonSongs.Count != currentSongs.Count || ethalonAlbum.AlbumDirectory != currentAlbum.AlbumDirectory)
				{
					ethalonAlbum.ContentIsIncorrect = currentAlbum.ContentIsIncorrect = true;
				}
				else
				{
					ethalonAlbum.ContentIsIncorrect = ethalonSongs.Any(s => s.ContentIsIncorrect);
					currentAlbum.ContentIsIncorrect = currentSongs.Any(s => s.ContentIsIncorrect);
				}
			}
		}

		private static void SetSongsCorrectness(int songNumber, SongTreeViewItem ethalonSong, SongTreeViewItem currentSong)
		{
			if (ethalonSong == null && currentSong == null)
			{
				throw new InvalidOperationException();
			}

			if (ethalonSong == null)
			{
				currentSong.ContentIsIncorrect = true;
			}
			else if (currentSong == null)
			{
				ethalonSong.ContentIsIncorrect = true;
			}
			else
			{
				currentSong.ContentIsIncorrect = ethalonSong.ContentIsIncorrect = (currentSong.Title != Invariant($"{songNumber:D2} - {ethalonSong.Title}.mp3"));
			}
		}

		private static void MarkAlbumSongsAsIncorrect(AlbumTreeViewItem album)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			foreach (var song in album.Songs)
			{
				song.ContentIsIncorrect = true;
			}
			album.ContentIsIncorrect = true;
		}
	}
}
