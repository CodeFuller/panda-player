using System;
using System.Collections.Generic;
using System.Linq;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongPlaylistViewModel : SongListViewModel, ISongPlaylistViewModel
	{
		private int currentSongIndex;
		private int CurrentSongIndex
		{
			get { return currentSongIndex; }
			set
			{
				currentSongIndex = value;
				Messenger.Default.Send(new PlaylistChangedEventArgs(this));
			}
		}

		private SongListItem CurrentItem => SongItems != null && CurrentSongIndex >= 0 && CurrentSongIndex < SongItems.Count ? SongItems[CurrentSongIndex] : null;

		public override bool DisplayTrackNumbers => false;

		public Song CurrentSong => CurrentItem?.Song;

		public Disc PlayedDisc
		{
			get
			{
				var discs = Songs.Select(s => s.Disc).Distinct().ToList();
				return discs.Count == 1 ? discs.Single() : null;
			}
		}

		public SongPlaylistViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
			: base(libraryContentUpdater, viewNavigator)
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
