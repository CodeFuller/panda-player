using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongListViewModel
	{
		bool DisplayTrackNumbers { get; }

		ObservableCollection<SongListItem> SongItems { get; }

		IEnumerable<Song> Songs { get; }

		SongListItem SelectedSongItem { get; }

		bool HasSongs { get; }

		int SongsNumber { get; }

		long TotalSongsFileSize { get; }

		TimeSpan TotalSongsDuration { get; }

		ICommand PlaySongsNextCommand { get; }
		ICommand PlaySongsLastCommand { get; }

		ICommand EditSongsPropertiesCommand { get; }

		IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		void SetSongs(IEnumerable<Song> newSongs);

		Task SetRatingForSelectedSongs(Rating rating);
	}
}
