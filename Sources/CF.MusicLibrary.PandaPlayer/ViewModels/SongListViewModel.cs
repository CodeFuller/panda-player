using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Enums;
using CF.Library.Core.Extensions;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
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
		private readonly IWindowService windowService;

		public abstract bool DisplayTrackNumbers { get; }

		private readonly ObservableCollection<SongListItem> songItems;

		public ReadOnlyObservableCollection<SongListItem> SongItems { get; }

		public IEnumerable<Song> Songs => SongItems.Select(s => s.Song);

		private SongListItem selectedSongItem;

		public SongListItem SelectedSongItem
		{
			get => selectedSongItem;
			set => Set(ref selectedSongItem, value);
		}

		public bool HasSongs => SongsNumber > 0;

		public int SongsNumber => Songs.Count();

		public long TotalSongsFileSize => Songs.Select(s => (long)s.FileSize).Sum();

		public TimeSpan TotalSongsDuration => Songs.Aggregate(TimeSpan.Zero, (currSum, currSong) => currSum + currSong.Duration);

		// Should be of type IList because of SelectedItem binding in SongListView
		private IList selectedSongItems;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Collection is used in two-way binding")]
		public IList SelectedSongItems
		{
			get => selectedSongItems;
			set => Set(ref selectedSongItems, value);
		}

		public IEnumerable<Song> SelectedSongs => SelectedSongItems.OfType<SongListItem>().Select(it => it.Song);

		public ICommand PlaySongsNextCommand { get; }

		public ICommand PlaySongsLastCommand { get; }

		public ICommand DeleteSongsFromDiscCommand { get; }

		public ICommand EditSongsPropertiesCommand { get; }

		public abstract ICommand PlayFromSongCommand { get; }

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		protected SongListViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator, IWindowService windowService)
		{
			this.libraryContentUpdater = libraryContentUpdater ?? throw new ArgumentNullException(nameof(libraryContentUpdater));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			songItems = new ObservableCollection<SongListItem>();
			SongItems = new ReadOnlyObservableCollection<SongListItem>(songItems);

			PlaySongsNextCommand = new RelayCommand(PlaySongsNext);
			PlaySongsLastCommand = new RelayCommand(PlaySongsLast);
			DeleteSongsFromDiscCommand = new AsyncRelayCommand(DeleteSongsFromDisc);
			EditSongsPropertiesCommand = new RelayCommand(EditSongsProperties);
			SetRatingMenuItems = RatingsHelper.AllowedRatingsDesc.Select(r => new SetRatingMenuItem(this, r)).ToList();
		}

		protected virtual void OnSongItemsChanged()
		{
			RaisePropertyChanged(nameof(HasSongs));
			RaisePropertyChanged(nameof(SongsNumber));
			RaisePropertyChanged(nameof(TotalSongsFileSize));
			RaisePropertyChanged(nameof(TotalSongsDuration));
		}

		internal async Task DeleteSongsFromDisc()
		{
			var selectedSongs = SelectedSongs.ToList();
			if (!selectedSongs.Any())
			{
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete {selectedSongs.Count} selected song(s)?", "Delete song(s)",
					ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Question) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			foreach (var song in selectedSongs)
			{
				await libraryContentUpdater.DeleteSong(song);
				for (var i = 0; i < songItems.Count;)
				{
					if (Object.ReferenceEquals(songItems[i].Song, song))
					{
						songItems.RemoveAt(i);
					}
					else
					{
						++i;
					}
				}
			}

			OnSongItemsChanged();
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
			songItems.Clear();
			AddSongs(newSongs);
		}

		protected void AddSongs(IEnumerable<Song> addedSongs)
		{
			songItems.AddRange(addedSongs.Select(song => new SongListItem(song)));
			OnSongItemsChanged();
		}

		protected void InsertSongs(int index, IEnumerable<Song> addedSongs)
		{
			foreach (var song in addedSongs)
			{
				songItems.Insert(index++, new SongListItem(song));
			}

			OnSongItemsChanged();
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
