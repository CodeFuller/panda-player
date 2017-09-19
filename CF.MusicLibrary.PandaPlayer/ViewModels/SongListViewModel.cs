using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;
		private readonly IViewNavigator viewNavigator;

		public abstract bool DisplayTrackNumbers { get; }

		private ObservableCollection<SongListItem> songItems;
		public ObservableCollection<SongListItem> SongItems
		{
			get { return songItems; }
			private set { Set(ref songItems, value); }
		}

		public IEnumerable<Song> Songs => SongItems.Select(s => s.Song);

		private SongListItem selectedSongItem;
		public SongListItem SelectedSongItem
		{
			get { return selectedSongItem; }
			set { Set(ref selectedSongItem, value); }
		}

		private IList selectedSongItems;
		public IList SelectedSongItems
		{
			get { return selectedSongItems; }
			set { Set(ref selectedSongItems, value); }
		}

		public IEnumerable<Song> SelectedSongs => SelectedSongItems.OfType<SongListItem>().Select(it => it.Song);

		public IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		public ICommand EditSongsPropertiesCommand { get; }

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

			SetRatingMenuItems = RatingsHelper.AllowedRatingsDesc.Select(r => new SetRatingMenuItem(this, r)).ToList();
			EditSongsPropertiesCommand = new RelayCommand(EditSongsProperties);
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
			SongItems = new ObservableCollection<SongListItem>(newSongs.Select(s => new SongListItem(s)));
		}

		public async Task SetRatingForSelectedSongs(Rating rating)
		{
			var updatedSongs = SelectedSongs.ToList();
			if (updatedSongs.Any())
			{
				await libraryContentUpdater.SetSongsRating(updatedSongs, rating);
			}
		}
	}
}
