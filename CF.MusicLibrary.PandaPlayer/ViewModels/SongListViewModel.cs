using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public abstract class SongListViewModel : ViewModelBase, ISongListViewModel
	{
		private readonly ILibraryContentUpdater libraryContentUpdater;

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

		protected SongListViewModel(ILibraryContentUpdater libraryContentUpdater)
		{
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.libraryContentUpdater = libraryContentUpdater;

			SongItems = new ObservableCollection<SongListItem>();

			SetRatingMenuItems = RatingsHelper.AllowedRatingsDesc.Select(r => new SetRatingMenuItem(this, r)).ToList();
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
