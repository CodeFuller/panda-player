using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.Player
{
	public class SongPlaylist : ViewModelBase, ISongPlaylist
	{
		private int CurrentSongIndex { get; set; }

		private ObservableCollection<SongListItem> songs;
		public ObservableCollection<SongListItem> Songs
		{
			get { return songs; }
			private set { Set(ref songs, value); }
		}

		private SongListItem CurrentItem => Songs != null && CurrentSongIndex >= 0 && CurrentSongIndex < Songs.Count ? Songs[CurrentSongIndex] : null;

		public Song CurrentSong => CurrentItem?.Song;

		public void SetSongs(IEnumerable<Song> newSongs)
		{
			Songs = new ObservableCollection<SongListItem>(newSongs.Select(s => new SongListItem(s)));
			CurrentSongIndex = -1;
		}

		public void SwitchToNextSong()
		{
			if (CurrentItem != null)
			{
				CurrentItem.IsCurrentlyPlayed = false;
			}

			++CurrentSongIndex;

			if (CurrentItem != null)
			{
				CurrentItem.IsCurrentlyPlayed = true;
			}
		}
	}
}
