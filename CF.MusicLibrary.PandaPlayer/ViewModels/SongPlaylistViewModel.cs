using System;
using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongPlaylistViewModel : SongListViewModel, ISongPlaylistViewModel
	{
		public override bool DisplayTrackNumbers => false;

		private int CurrentSongIndex { get; set; }

		private SongListItem CurrentItem => SongItems != null && CurrentSongIndex >= 0 && CurrentSongIndex < SongItems.Count ? SongItems[CurrentSongIndex] : null;

		public Song CurrentSong => CurrentItem?.Song;

		public SongPlaylistViewModel(ILibraryContentUpdater libraryContentUpdater)
			: base(libraryContentUpdater)
		{
		}

		public override void SetSongs(IEnumerable<Song> newSongs)
		{
			base.SetSongs(newSongs);
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

		public void SwitchToSong(Song song)
		{
			if (CurrentItem != null)
			{
				CurrentItem.IsCurrentlyPlayed = false;
			}

			CurrentSongIndex = GetSongIndex(song);

			if (CurrentItem != null)
			{
				CurrentItem.IsCurrentlyPlayed = true;
			}
		}

		private int GetSongIndex(Song song)
		{
			for (var i = 0; i < SongItems.Count; ++i)
			{
				if (SongItems[i].Song == song)
				{
					return i;
				}
			}

			throw new InvalidOperationException("Failed to find song in the list");
		}
	}
}
