using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
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

		ICommand DeleteSongsFromDiscCommand { get; }

		ICommand EditSongsPropertiesCommand { get; }

		IReadOnlyCollection<SetRatingMenuItem> SetRatingMenuItems { get; }

		void SetSongs(IEnumerable<SongModel> newSongs);

		Task SetRatingForSelectedSongs(RatingModel rating, CancellationToken cancellationToken);
	}
}
