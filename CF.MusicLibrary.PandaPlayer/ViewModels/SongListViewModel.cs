using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		public ICommand PlayDiscFromSongCommand { get; }

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
			PlayDiscFromSongCommand = new RelayCommand(PlayDiscFromSong);
		}

		public void SetSongs(IEnumerable<Song> newSongs)
		{
			Songs = new ObservableCollection<Song>(newSongs);
		}

		private void PlayDiscFromSong()
		{
			Messenger.Default.Send(new PlayDiscFromSongEventArgs(SelectedSong));
		}
	}
}
