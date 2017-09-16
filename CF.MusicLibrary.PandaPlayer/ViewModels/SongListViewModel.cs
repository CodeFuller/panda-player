using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

		public abstract bool DisplayTrackNumbers { get; }

		private ObservableCollection<SongListItem> songItems;
		public ObservableCollection<SongListItem> SongItems
		{
			get { return songItems; }
			private set { Set(ref songItems, value); }
		}

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

			SetRatingMenuItems = Enum.GetValues(typeof(Rating)).Cast<Rating>().Where(r => r != Rating.Invalid)
				.OrderByDescending(r => r).Select(r => new SetRatingMenuItem(this, r)).ToList();
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
				foreach (var song in updatedSongs)
				{
					song.Rating = rating;
				}

				await libraryContentUpdater.UpdateSongs(updatedSongs, UpdatedSongProperties.Rating);
			}
		}
	}
}
