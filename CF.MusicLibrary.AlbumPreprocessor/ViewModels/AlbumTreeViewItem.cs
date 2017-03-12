using System;
using System.Collections.ObjectModel;
using System.Linq;
using static System.FormattableString;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AlbumTreeViewItem : EditableTreeViewItem
	{
		private readonly SongTreeViewItem songSeparator;

		public AlbumContent Album { get; }

		public ReadOnlyCollection<SongTreeViewItem> Songs { get; }

		private readonly MainWindowModel host;

		public AlbumTreeViewItem(MainWindowModel host, AlbumContent album)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			this.host = host;
			Album = album;
			songSeparator = new SongTreeViewItem(this, new SongContent(String.Empty));
			Songs = new ReadOnlyCollection<SongTreeViewItem>(album.Songs.Select(s => new SongTreeViewItem(this, s)).Concat(Enumerable.Repeat(songSeparator, 1)) .ToList());
		}

		public void ReloadContent()
		{
			host.ReloadAlbum(this);
		}

		public void SetSongsCorrectness(AlbumContent ethalonAlbumContent)
		{
			for (var i = 0; i < Songs.Count; ++i)
			{
				if (Songs[i] != songSeparator)
				{
					Songs[i].ContentIsIncorrect = (ethalonAlbumContent == null || i >= ethalonAlbumContent.Songs.Count || Songs[i].Title != Invariant($"{i + 1:D2} - {ethalonAlbumContent.Songs[i].Title}.mp3"));
				}
			}
		}
	}
}
