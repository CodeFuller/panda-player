using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private ObservableCollection<Song> songs;
		public ObservableCollection<Song> Songs
		{
			get { return songs; }
			private set { Set(ref songs, value); }
		}

		private Song selectedSong;
		public Song SelectedSong
		{
			get { return selectedSong; }
			set { Set(ref selectedSong, value); }
		}

		public SongListViewModel()
		{
			Songs = new ObservableCollection<Song>();
		}

		public void SetSongs(IEnumerable<Song> newSongs)
		{
			Songs = new ObservableCollection<Song>(newSongs);
		}
	}
}
