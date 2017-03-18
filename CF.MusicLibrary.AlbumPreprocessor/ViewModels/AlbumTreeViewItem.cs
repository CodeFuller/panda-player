using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CF.Library.Core.Extensions;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AlbumTreeViewItem : EditableTreeViewItem
	{
		private readonly AlbumTreeViewModel host;

		public ReadOnlyCollection<SongTreeViewItem> SongItems { get; }

		public IEnumerable<SongTreeViewItem> Songs => SongItems.Take(SongItems.Count - 1).ToCollection();

		private string albumDirectory;
		public string AlbumDirectory
		{
			get { return albumDirectory; }
			set
			{
				if (albumDirectory != null)
				{
					//	Exception, thrown at this point, won't blow up,
					//	because exceptions thrown from binding properties are treated as validation failures.
					//	http://stackoverflow.com/questions/12658220/exceptions-thrown-during-a-set-operation-in-a-property-are-not-being-caught
					//	http://stackoverflow.com/questions/1488472/best-practices-throwing-exceptions-from-properties
					//	It's not a big problem because directory title will not be updated and still will be marked as incorrect.
					Directory.Move(albumDirectory, value);
					ReloadContent();
				}

				Set(ref albumDirectory, value);
			}
		}

		private bool contentIsIncorrect;
		public bool ContentIsIncorrect
		{
			get { return contentIsIncorrect; }
			set
			{
				Set(ref contentIsIncorrect, value);
			}
		}

		public AlbumTreeViewItem(AlbumTreeViewModel host, AlbumContent album)
		{
			if (album == null)
			{
				throw new ArgumentNullException(nameof(album));
			}

			AlbumDirectory = album.AlbumDirectory;

			this.host = host;
			var songSeparator = new SongTreeViewItem(this, new SongContent(String.Empty));
			SongItems = new ReadOnlyCollection<SongTreeViewItem>(album.Songs.Select(s => new SongTreeViewItem(this, s)).Concat(Enumerable.Repeat(songSeparator, 1)).ToList());
		}

		public void ReloadContent()
		{
			host.ReloadAlbum(this);
		}
	}
}
