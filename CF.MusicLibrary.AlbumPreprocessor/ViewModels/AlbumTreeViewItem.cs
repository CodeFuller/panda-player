using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AlbumTreeViewItem : EditableTreeViewItem
	{
		public AlbumContent Album { get; }

		public ReadOnlyCollection<SongTreeViewItem> SongItems { get; }

		public IEnumerable<SongTreeViewItem> Songs => SongItems.Take(SongItems.Count - 1).ToCollection();

		private readonly AlbumTreeViewModel host;

		public bool ContentIsIncorrect => Songs.Any(s => s.ContentIsIncorrect);

		public AlbumTreeViewItem(AlbumTreeViewModel host, AlbumContent album)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			this.host = host;
			Album = album;
			var songSeparator = new SongTreeViewItem(this, new SongContent(String.Empty));
			SongItems = new ReadOnlyCollection<SongTreeViewItem>(album.Songs.Select(s => new SongTreeViewItem(this, s)).Concat(Enumerable.Repeat(songSeparator, 1)) .ToList());
		}

		public void ReloadContent()
		{
			host.ReloadAlbum(this);
		}
	}
}
