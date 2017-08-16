using System.Collections.Generic;
using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Player
{
	public interface ISongPlaylist
	{
		ObservableCollection<SongListItem> Songs { get; }

		Song CurrentSong { get; }

		void SetSongs(IEnumerable<Song> newSongs);

		void SwitchToNextSong();
	}
}
