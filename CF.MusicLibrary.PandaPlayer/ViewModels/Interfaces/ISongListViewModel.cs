﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongListViewModel
	{
		ObservableCollection<Song> Songs { get; }

		Song SelectedSong { get; }

		void SetSongs(IEnumerable<Song> newSongs);

		Task SetRatingForSelectedSongs(Rating rating);
	}
}
