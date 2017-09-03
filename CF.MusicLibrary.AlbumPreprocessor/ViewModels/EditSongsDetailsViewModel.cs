using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditSongsDetailsViewModel : ViewModelBase, IEditSongsDetailsViewModel
	{
		public string Name => "Edit Songs Details";

		public bool DataIsReady => Songs.Any();

		public ObservableCollection<SongTagDataViewItem> Songs { get; private set; }

		public void SetSongs(IEnumerable<TaggedSongData> songs)
		{
			Songs = new ObservableCollection<SongTagDataViewItem>(songs.Select(s => new SongTagDataViewItem(s)));
		}
	}
}
