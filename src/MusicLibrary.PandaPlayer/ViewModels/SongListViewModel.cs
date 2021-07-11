using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Internal;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.ViewModels.Internal;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Shared.Extensions;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly IViewNavigator viewNavigator;

		protected ISongsService SongsService { get; }

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

		public TimeSpan TotalSongsDuration => Songs.Aggregate(TimeSpan.Zero, (currentSum, currentSong) => currentSum + currentSong.Duration);

		// Should be of type IList because of SelectedItem binding in SongListView
		private IList selectedSongItems;

#pragma warning disable CA2227 // Collection properties should be read only - Collection is used in two-way binding
		public IList SelectedSongItems
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => selectedSongItems;
			set => Set(ref selectedSongItems, value);
		}

		public IEnumerable<SongModel> SelectedSongs => SelectedSongItems?.OfType<SongListItem>().Select(it => it.Song) ?? Enumerable.Empty<SongModel>();

		public abstract ICommand PlaySongsNextCommand { get; }

		public abstract ICommand PlaySongsLastCommand { get; }

		public ICommand EditSongsPropertiesCommand { get; }

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		protected SongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
		{
			this.SongsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			songItems = new ObservableCollection<SongListItem>();
			SongItems = new ReadOnlyObservableCollection<SongListItem>(songItems);

			EditSongsPropertiesCommand = new AsyncRelayCommand(() => EditSongsProperties(CancellationToken.None));
			SetRatingMenuItems = RatingHelpers.AllRatingValues
				.OrderByDescending(r => r)
				.Select(r => new SetRatingMenuItem(SetRatingForSelectedSongs, r))
				.ToList();

			Messenger.Default.Register<SongChangedEventArgs>(this, e => OnSongChanged(e.Song, e.PropertyName));
			Messenger.Default.Register<DiscChangedEventArgs>(this, e => OnDiscChanged(e.Disc, e.PropertyName));
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		private async Task EditSongsProperties(CancellationToken cancellationToken)
		{
			var selectedSongs = SelectedSongs.ToList();
			if (selectedSongs.Any())
			{
				await viewNavigator.ShowSongPropertiesView(selectedSongs, cancellationToken);
			}
		}

		public void SetSongs(IEnumerable<SongModel> newSongs)
		{
			songItems.Clear();
			songItems.AddRange(newSongs.Select(song => new SongListItem(song)));

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

		private async Task SetRatingForSelectedSongs(RatingModel rating, CancellationToken cancellationToken)
		{
			foreach (var song in SelectedSongs.ToList())
			{
				song.Rating = rating;
				await SongsService.UpdateSong(song, cancellationToken);
			}
		}

		private void OnSongChanged(SongModel changedSong, string propertyName)
		{
			// ToList() call is required to prevent exception "Collection was modified; enumeration operation may not execute."
			foreach (var song in GetSongsForUpdate(changedSong).ToList())
			{
				SongUpdater.UpdateSong(changedSong, song, propertyName);
			}

			if (!(propertyName == nameof(SongModel.DeleteDate) && changedSong.IsDeleted))
			{
				return;
			}

			var songItemsChanged = false;

			// The same song could appear several times in the list, so we check the whole list and remove all instances.
			for (var i = 0; i < songItems.Count;)
			{
				var song = songItems[i].Song;
				if (song.Id != changedSong.Id)
				{
					++i;
					continue;
				}

				songItems.RemoveAt(i);
				songItemsChanged = true;
			}

			if (songItemsChanged)
			{
				OnSongItemsChanged();
			}
		}

		protected void RemoveSongItems(IEnumerable<SongListItem> songItemsToRemove)
		{
			foreach (var songItem in songItemsToRemove)
			{
				songItems.Remove(songItem);
			}

			OnSongItemsChanged();
		}

		private void OnSongItemsChanged()
		{
			RaisePropertyChanged(nameof(HasSongs));
			RaisePropertyChanged(nameof(SongsNumber));
			RaisePropertyChanged(nameof(TotalSongsFileSize));
			RaisePropertyChanged(nameof(TotalSongsDuration));
		}

		private void OnDiscChanged(DiscModel changedDisc, string propertyName)
		{
			foreach (var disc in GetDiscsForUpdate(changedDisc))
			{
				DiscUpdater.UpdateDisc(changedDisc, disc, propertyName);
			}
		}

		private void OnDiscImageChanged(DiscModel changedDisc)
		{
			foreach (var disc in GetDiscsForUpdate(changedDisc))
			{
				disc.Images = changedDisc.Images;
			}
		}

		private IEnumerable<SongModel> GetSongsForUpdate(SongModel changedSong)
		{
			return Songs
				.Where(s => s.Id == changedSong.Id)
				.Where(s => !Object.ReferenceEquals(s, changedSong));
		}

		private IEnumerable<DiscModel> GetDiscsForUpdate(DiscModel changedDisc)
		{
			return Songs
				.Select(s => s.Disc)
				.Where(d => d.Id == changedDisc.Id)
				.Where(d => !Object.ReferenceEquals(d, changedDisc))
				.Distinct();
		}
	}
}
