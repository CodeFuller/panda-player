using System;
using System.Collections.ObjectModel;
using CF.MusicLibrary.AlbumPreprocessor.Interfaces;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public class AlbumContentComparer : IAlbumContentComparer
	{
		public void SetAlbumsCorrectness(Collection<AlbumContent> ethalonAlbums, Collection<AlbumTreeViewItem> currentAlbums)
		{
			if (ethalonAlbums == null)
			{
				throw new ArgumentNullException(nameof(ethalonAlbums));
			}
			if (currentAlbums == null)
			{
				throw new ArgumentNullException(nameof(currentAlbums));
			}

			for (var i = 0; i < currentAlbums.Count; ++i)
			{
				currentAlbums[i].SetSongsCorrectness(i < ethalonAlbums.Count ? ethalonAlbums[i] : null);
			}
		}
	}
}
