using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.AlbumPreprocessor.AddingToLibrary;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.AlbumPreprocessor.ViewModels
{
	public class EditSongsDetailsViewModel : ViewModelBase
	{
		public ObservableCollection<TaggedSongData> Songs { get; private set; }

		public void SetSongs(IEnumerable<TaggedSongData> songs)
		{
			Songs = new ObservableCollection<TaggedSongData>(songs);
		}
	}
}
