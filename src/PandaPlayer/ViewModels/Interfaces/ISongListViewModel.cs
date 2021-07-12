using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ISongListViewModel
	{
		bool DisplayTrackNumbers { get; }

		ReadOnlyObservableCollection<SongListItem> SongItems { get; }

		IEnumerable<SongModel> Songs { get; }

		SongListItem SelectedSongItem { get; }

		bool HasSongs { get; }

		int SongsNumber { get; }

		long TotalSongsFileSize { get; }

		TimeSpan TotalSongsDuration { get; }

		ICommand PlaySongsNextCommand { get; }

		ICommand PlaySongsLastCommand { get; }

		ICommand EditSongsPropertiesCommand { get; }

		IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		void SetSongs(IEnumerable<SongModel> newSongs);
	}
}
