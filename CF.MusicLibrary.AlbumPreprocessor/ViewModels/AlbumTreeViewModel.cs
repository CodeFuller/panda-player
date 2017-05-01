using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class AlbumTreeViewModel : ViewModelBase, IEnumerable<AlbumTreeViewItem>
	{
		private readonly MainWindowModel host;

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

		public AlbumTreeViewModel(MainWindowModel host)
		{
			this.host = host;
			Albums = new Collection<AlbumTreeViewItem>();
		}

		public void SetAlbums(IEnumerable<AlbumContent> newAlbums)
		{
			Albums = newAlbums.Select(a => new AlbumTreeViewItem(this, a)).ToCollection();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameter use makes sense in method semantics.")]
		public void ReloadAlbum(AlbumTreeViewItem album)
		{
			host.LoadCurrentAlbums();
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
