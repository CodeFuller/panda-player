using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;
		private readonly IEditSongPropertiesViewModel editSongPropertiesViewModel;
		private readonly IWindowService windowService;

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

		protected SongListViewModel(ILibraryContentUpdater libraryContentUpdater, IEditSongPropertiesViewModel editSongPropertiesViewModel, IWindowService windowService)
		{
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}
			if (editSongPropertiesViewModel == null)
			{
				throw new ArgumentNullException(nameof(editSongPropertiesViewModel));
			}
			if (windowService == null)
			{
				throw new ArgumentNullException(nameof(windowService));
			}

			this.libraryContentUpdater = libraryContentUpdater;
			this.editSongPropertiesViewModel = editSongPropertiesViewModel;
			this.windowService = windowService;

			SongItems = new ObservableCollection<SongListItem>();

			SetRatingMenuItems = RatingsHelper.AllowedRatingsDesc.Select(r => new SetRatingMenuItem(this, r)).ToList();
			EditSongsPropertiesCommand = new AsyncRelayCommand(EditSongsProperties);
		}

		private async Task EditSongsProperties()
		{
			var selectedSongs = SelectedSongs.ToList();
			if (!selectedSongs.Any())
			{
				return;
			}

			editSongPropertiesViewModel.Load(selectedSongs);
			if (windowService.ShowSongPropertiesView(editSongPropertiesViewModel))
			{
				//	Should we rename a song?
				if (selectedSongs.Count == 1)
				{
					Uri newSongUri = editSongPropertiesViewModel.UpdatedSongUri;
					if (newSongUri != null)
					{
						await libraryContentUpdater.ChangeSongUri(selectedSongs.Single(), newSongUri);
					}
				}

				await libraryContentUpdater.UpdateSongs(editSongPropertiesViewModel.GetUpdatedSongs(), UpdatedSongProperties.ForceTagUpdate);
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
