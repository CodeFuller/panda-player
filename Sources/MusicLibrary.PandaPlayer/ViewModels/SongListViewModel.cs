using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Enums;
using CF.Library.Core.Extensions;
using CF.Library.Core.Interfaces;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ISongsService songsService;
		private readonly IViewNavigator viewNavigator;
		private readonly IWindowService windowService;

		public abstract bool DisplayTrackNumbers { get; }

		private readonly ObservableCollection<SongListItem> songItems;

		public ReadOnlyObservableCollection<SongListItem> SongItems { get; }

		public IEnumerable<SongModel> Songs => SongItems.Select(s => s.Song);

		private SongListItem selectedSongItem;

		public SongListItem SelectedSongItem
		{
			get => selectedSongItem;
			set => Set(ref selectedSongItem, value);
		}

		public bool HasSongs => SongsNumber > 0;

		public int SongsNumber => Songs.Count();

		public long TotalSongsFileSize => Songs.Select(s => s.Size ?? 0).Sum();

		public TimeSpan TotalSongsDuration => Songs.Aggregate(TimeSpan.Zero, (currSum, currSong) => currSum + currSong.Duration);

		// Should be of type IList because of SelectedItem binding in SongListView
		private IList selectedSongItems;

#pragma warning disable CA2227 // Collection properties should be read only - Collection is used in two-way binding
		public IList SelectedSongItems
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => selectedSongItems;
			set => Set(ref selectedSongItems, value);
		}

		public IEnumerable<SongModel> SelectedSongs => SelectedSongItems.OfType<SongListItem>().Select(it => it.Song);

		public ICommand PlaySongsNextCommand { get; }

		public ICommand PlaySongsLastCommand { get; }

		public ICommand DeleteSongsFromDiscCommand { get; }

		public ICommand EditSongsPropertiesCommand { get; }

		public abstract ICommand PlayFromSongCommand { get; }

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		protected SongListViewModel(ISongsService songsService, IViewNavigator viewNavigator, IWindowService windowService)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			songItems = new ObservableCollection<SongListItem>();
			SongItems = new ReadOnlyObservableCollection<SongListItem>(songItems);

			PlaySongsNextCommand = new RelayCommand(PlaySongsNext);
			PlaySongsLastCommand = new RelayCommand(PlaySongsLast);
			DeleteSongsFromDiscCommand = new AsyncRelayCommand(() => DeleteSongsFromDisc(CancellationToken.None));
			EditSongsPropertiesCommand = new AsyncRelayCommand(EditSongsProperties);
			SetRatingMenuItems = RatingModel.All.Select(r => new SetRatingMenuItem(this, r)).ToList();
		}

		protected virtual void OnSongItemsChanged()
		{
			RaisePropertyChanged(nameof(HasSongs));
			RaisePropertyChanged(nameof(SongsNumber));
			RaisePropertyChanged(nameof(TotalSongsFileSize));
			RaisePropertyChanged(nameof(TotalSongsDuration));
		}

		internal async Task DeleteSongsFromDisc(CancellationToken cancellationToken)
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
				await songsService.DeleteSong(song, cancellationToken);
				for (var i = 0; i < songItems.Count;)
				{
					if (songItems[i].Song.Id == song.Id)
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

		private async Task EditSongsProperties()
		{
			var selectedSongs = SelectedSongs.ToList();
			if (selectedSongs.Any())
			{
				await viewNavigator.ShowSongPropertiesView(selectedSongs, CancellationToken.None);
			}
		}

		public virtual void SetSongs(IEnumerable<SongModel> newSongs)
		{
			songItems.Clear();
			AddSongs(newSongs);
		}

		protected void AddSongs(IEnumerable<SongModel> addedSongs)
		{
			songItems.AddRange(addedSongs.Select(song => new SongListItem(song)));
			OnSongItemsChanged();
		}

		protected void InsertSongs(int index, IEnumerable<SongModel> addedSongs)
		{
			foreach (var song in addedSongs)
			{
				songItems.Insert(index++, new SongListItem(song));
			}

			OnSongItemsChanged();
		}

		public async Task SetRatingForSelectedSongs(RatingModel rating, CancellationToken cancellationToken)
		{
			var updatedSongs = SelectedSongs.ToList();
			if (updatedSongs.Any())
			{
				foreach (var song in updatedSongs)
				{
					song.Rating = rating;
					await songsService.UpdateSong(song, cancellationToken);
				}
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

		private void AddSongsToPlaylist<TAddingSongsToPlaylistEventArgs>(Func<IEnumerable<SongModel>, TAddingSongsToPlaylistEventArgs> eventFactory)
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
