using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
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

		private readonly ILibraryContentUpdater libraryContentUpdater;

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

		private IList selectedItems;
		public IList SelectedItems
		{
			get { return selectedItems; }
			set { Set(ref selectedItems, value); }
		}

		public IEnumerable<Song> SelectedSongs => SelectedItems.OfType<Song>();

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		public SongListViewModel(ILibraryContentUpdater libraryContentUpdater)
		{
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.libraryContentUpdater = libraryContentUpdater;

			Songs = new ObservableCollection<Song>();
			PlayDiscFromSongCommand = new RelayCommand(PlayDiscFromSong);

			SetRatingMenuItems = Enum.GetValues(typeof(Rating)).Cast<Rating>().Where(r => r != Rating.Invalid)
				.OrderByDescending(r => r).Select(r => new SetRatingMenuItem(this, r)).ToList();
		}

		public void SetSongs(IEnumerable<Song> newSongs)
		{
			Songs = new ObservableCollection<Song>(newSongs);
		}

		public async Task SetRatingForSelectedSongs(Rating rating)
		{
			var updatedSongs = SelectedSongs.ToList();
			if (updatedSongs.Any())
			{
				foreach (var song in updatedSongs)
				{
					song.Rating = rating;
				}

				await libraryContentUpdater.UpdateSongs(updatedSongs, UpdatedSongProperties.Rating);
			}
		}

		private void PlayDiscFromSong()
		{
			Messenger.Default.Send(new PlayDiscFromSongEventArgs(SelectedSong));
		}
	}
}
