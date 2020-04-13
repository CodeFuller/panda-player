using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;
using MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;

namespace MusicLibrary.DiscPreprocessor.ViewModels
{
	public class EditSongsDetailsViewModel : ViewModelBase, IEditSongsDetailsViewModel
	{
		public string Name => "Edit Songs Details";

		public bool DataIsReady => Songs.Any();

		public ObservableCollection<SongViewItem> Songs { get; private set; }

		public void SetSongs(IEnumerable<AddedSong> songs)
		{
			Songs = new ObservableCollection<SongViewItem>(songs.Select(s => new SongViewItem(s)));
		}
	}
}
