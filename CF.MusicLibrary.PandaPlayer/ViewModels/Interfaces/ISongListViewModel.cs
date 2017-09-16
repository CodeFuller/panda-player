using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongListViewModel
	{
		bool DisplayTrackNumbers { get; }

		ObservableCollection<SongListItem> SongItems { get; }

		IEnumerable<Song> Songs { get; }

		SongListItem SelectedSongItem { get; }

		void SetSongs(IEnumerable<Song> newSongs);

		Task SetRatingForSelectedSongs(Rating rating);
	}
}
