using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel
	{
		ObservableCollection<SongListItem> SongItems { get; }

		Song CurrentSong { get; }

		void SetSongs(IEnumerable<Song> newSongs);

		void SwitchToNextSong();

		void SwitchToSong(Song song);
	}
}
