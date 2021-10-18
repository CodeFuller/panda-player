using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Internal;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Shared;
using PandaPlayer.Shared.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Internal;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ISongsService songsService;

		protected IViewNavigator ViewNavigator { get; }

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

		private IList selectedSongItems;

#pragma warning disable CA2227 // Collection properties should be read only - Collection is used in two-way binding
		public IList SelectedSongItems
#pragma warning restore CA2227 // Collection properties should be read only
		{
			get => selectedSongItems;
			set => Set(ref selectedSongItems, value);
		}

		protected IEnumerable<SongModel> SelectedSongs => SelectedSongItems?.OfType<SongListItem>().Select(it => it.Song) ?? Enumerable.Empty<SongModel>();

		public bool HasSongs => SongsNumber > 0;

		public int SongsNumber => Songs.Count();

		public string TotalSongsFileSize => Songs.All(x => x.IsDeleted) ? "N/A" : FileSizeFormatter.GetFormattedFileSize(Songs.Select(s => s.Size ?? 0).Sum());

		public TimeSpan TotalSongsDuration => Songs.Aggregate(TimeSpan.Zero, (currentSum, currentSong) => currentSum + currentSong.Duration);

		protected SongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			ViewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			songItems = new ObservableCollection<SongListItem>();
			SongItems = new ReadOnlyObservableCollection<SongListItem>(songItems);

			Messenger.Default.Register<SongChangedEventArgs>(this, e => OnSongChanged(e.Song, e.PropertyName));
			Messenger.Default.Register<DiscChangedEventArgs>(this, e => OnDiscChanged(e.Disc, e.PropertyName));
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		protected Task EditSongsProperties(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			return ViewNavigator.ShowSongPropertiesView(songs, cancellationToken);
		}

		protected ExpandableMenuItem GetSetRatingContextMenuItem(IEnumerable<SongModel> songs)
		{
			return new()
			{
				Header = "Set Rating",
				IconKind = PackIconKind.Star,
				Items = RatingHelpers.AllRatingValues
					.OrderByDescending(r => r)
					.Select(rating => new SetRatingMenuItem(rating)
					{
						Command = new AsyncRelayCommand(() => SetRatingForSongs(songs, rating, CancellationToken.None)),
					}),
			};
		}

		internal void SetSongs(IEnumerable<SongModel> newSongs)
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

		private async Task SetRatingForSongs(IEnumerable<SongModel> songs, RatingModel rating, CancellationToken cancellationToken)
		{
			foreach (var song in songs)
			{
				song.Rating = rating;
				await songsService.UpdateSong(song, cancellationToken);
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
