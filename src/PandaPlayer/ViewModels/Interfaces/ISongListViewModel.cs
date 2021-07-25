using System;
using System.Collections;
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

		SongListItem SelectedSongItem { get; set; }

		// Should be of type IList because of SelectedItem binding in SongListView
#pragma warning disable CA2227 // Collection properties should be read only - Collection is used in two-way binding
		IList SelectedSongItems { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		bool HasSongs { get; }

		int SongsNumber { get; }

		long TotalSongsFileSize { get; }

		TimeSpan TotalSongsDuration { get; }

		ICommand PlaySongsNextCommand { get; }

		ICommand PlaySongsLastCommand { get; }

		ICommand EditSongsPropertiesCommand { get; }

		IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }
	}
}
