using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditSongsDetailsViewModel : ViewModelBase
	{
		public virtual ObservableCollection<SongTagDataViewItem> Songs { get; private set; }

		public virtual void SetSongs(IEnumerable<TaggedSongData> songs)
		{
			Songs = new ObservableCollection<SongTagDataViewItem>(songs.Select(s => new SongTagDataViewItem(s)));
		}
	}
}
