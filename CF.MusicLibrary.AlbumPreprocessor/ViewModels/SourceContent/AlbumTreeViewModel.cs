using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels.SourceContent
{
	public class AlbumTreeViewModel : ViewModelBase, IEnumerable<AlbumTreeViewItem>
	{
		private Collection<AlbumTreeViewItem> albums;
		public Collection<AlbumTreeViewItem> Albums
		{
			get { return albums; }
			set
			{
				Set(ref albums, value);
			}
		}

		public bool ContentIsIncorrect => Albums.Any(s => s.ContentIsIncorrect);

		public AlbumTreeViewModel()
		{
			Albums = new Collection<AlbumTreeViewItem>();
		}

		public void SetAlbums(IEnumerable<AlbumContent> newAlbums)
		{
			Albums = newAlbums.Select(a => new AlbumTreeViewItem(a)).ToCollection();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<AlbumTreeViewItem> GetEnumerator()
		{
			return Albums.GetEnumerator();
		}
	}
}
