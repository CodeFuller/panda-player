using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;
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
		protected ISongsService SongsService { get; }

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

		public abstract IEnumerable<BasicMenuItem> ContextMenuItems { get; }

		protected SongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
		{
			SongsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			ViewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			songItems = new ObservableCollection<SongListItem>();
			SongItems = new ReadOnlyObservableCollection<SongListItem>(songItems);
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
					.Select(rating => new SetRatingMenuItem(rating, () => SetRatingForSongs(songs, rating, CancellationToken.None))),
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
			void UpdateSong(SongModel song)
			{
				song.Rating = rating;
			}

			foreach (var song in songs)
			{
				await SongsService.UpdateSong(song, UpdateSong, cancellationToken);
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
	}
}
