using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;
		private readonly IViewNavigator viewNavigator;

		public abstract bool DisplayTrackNumbers { get; }

		public ObservableCollection<SongListItem> SongItems { get; }

		public IEnumerable<Song> Songs => SongItems.Select(s => s.Song);

		private SongListItem selectedSongItem;
		public SongListItem SelectedSongItem
		{
			get { return selectedSongItem; }
			set { Set(ref selectedSongItem, value); }
		}

		public bool HasSongs => SongsNumber > 0;

		public int SongsNumber => Songs.Count();

		public long TotalSongsFileSize => Songs.Select(s => (long)s.FileSize).Sum();

		public TimeSpan TotalSongsDuration => Songs.Aggregate(TimeSpan.Zero, (currSum, currSong) => currSum + currSong.Duration);

		private IList selectedSongItems;
		public IList SelectedSongItems
		{
			get { return selectedSongItems; }
			set { Set(ref selectedSongItems, value); }
		}

		public IEnumerable<Song> SelectedSongs => SelectedSongItems.OfType<SongListItem>().Select(it => it.Song);

		public ICommand PlaySongsNextCommand { get; }
		public ICommand PlaySongsLastCommand { get; }

		public ICommand EditSongsPropertiesCommand { get; }

		public abstract ICommand PlayFromSongCommand { get; }

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		protected SongListViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
		{
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}

			this.libraryContentUpdater = libraryContentUpdater;
			this.viewNavigator = viewNavigator;

			SongItems = new ObservableCollection<SongListItem>();
			SongItems.CollectionChanged += SongItems_CollectionChanged;

			PlaySongsNextCommand = new RelayCommand(PlaySongsNext);
			PlaySongsLastCommand = new RelayCommand(PlaySongsLast);
			EditSongsPropertiesCommand = new RelayCommand(EditSongsProperties);
			SetRatingMenuItems = RatingsHelper.AllowedRatingsDesc.Select(r => new SetRatingMenuItem(this, r)).ToList();
		}

		private void SongItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(HasSongs));
			RaisePropertyChanged(nameof(SongsNumber));
			RaisePropertyChanged(nameof(TotalSongsFileSize));
			RaisePropertyChanged(nameof(TotalSongsDuration));
		}

		private void EditSongsProperties()
		{
			var selectedSongs = SelectedSongs.ToList();
			if (selectedSongs.Any())
			{
				viewNavigator.ShowSongPropertiesView(selectedSongs);
			}
		}

		public virtual void SetSongs(IEnumerable<Song> newSongs)
		{
			SongItems.Clear();
			foreach (var song in newSongs)
			{
				SongItems.Add(new SongListItem(song));
			}
		}

		public async Task SetRatingForSelectedSongs(Rating rating)
		{
			var updatedSongs = SelectedSongs.ToList();
			if (updatedSongs.Any())
			{
				await libraryContentUpdater.SetSongsRating(updatedSongs, rating);
			}
		}

		internal void PlaySongsNext()
		{
			AddSongsToPlaylist(songs => new AddingSongsToPlaylistNextEventArgs(songs));
		}

		internal void PlaySongsLast()
		{
			AddSongsToPlaylist(songs => new AddingSongsToPlaylistLastEventArgs(songs));
		}

		private void AddSongsToPlaylist<TAddingSongsToPlaylistEventArgs>(Func<IEnumerable<Song>, TAddingSongsToPlaylistEventArgs> eventFactory)
			where TAddingSongsToPlaylistEventArgs : AddingSongsToPlaylistEventArgs
		{
			var selectedSongs = SelectedSongs.ToList();
			if (selectedSongs.Any())
			{
				Messenger.Default.Send(eventFactory(selectedSongs));
			}
		}
	}
}
