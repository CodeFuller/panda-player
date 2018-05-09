using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.Events.SongEvents;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongPlaylistViewModel : SongListViewModel, ISongPlaylistViewModel
	{
		private int? currentSongIndex;
		public int? CurrentSongIndex
		{
			get => currentSongIndex;
			protected set
			{
				if (currentSongIndex == value)
				{
					return;
				}

				if (CurrentItem != null)
				{
					CurrentItem.IsCurrentlyPlayed = false;
				}

				currentSongIndex = value < SongItems.Count ? value : null;

				if (CurrentItem != null)
				{
					CurrentItem.IsCurrentlyPlayed = true;
				}
			}
		}

		private SongListItem CurrentItem => SongItems != null && CurrentSongIndex != null ? SongItems[CurrentSongIndex.Value] : null;

		public override bool DisplayTrackNumbers => false;

		public Song CurrentSong => CurrentItem?.Song;

		public Disc PlayedDisc => Songs.Select(s => s.Disc).UniqueOrDefault();

		public override ICommand PlayFromSongCommand { get; }

		public ICommand NavigateToSongDiscCommand { get; }

		public SongPlaylistViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
			: base(libraryContentUpdater, viewNavigator)
		{
			PlayFromSongCommand = new RelayCommand(PlayFromSong);
			NavigateToSongDiscCommand = new RelayCommand(NavigateToSongDisc);

			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => OnAddingNextSongs(e.Songs));
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => OnAddingLastSongs(e.Songs));
		}

		protected virtual void OnPlaylistChanged()
		{
			Messenger.Default.Send(new PlaylistChangedEventArgs(this));
		}

		public override void SetSongs(IEnumerable<Song> newSongs)
		{
			CurrentSongIndex = null;
			SetSongsRaw(newSongs);
			OnPlaylistChanged();
		}

		protected void SetSongsRaw(IEnumerable<Song> newSongs)
		{
			base.SetSongs(newSongs);
		}

		public void SwitchToNextSong()
		{
			CurrentSongIndex = CurrentSongIndex + 1 ?? 0;
			OnPlaylistChanged();
		}

		public void SwitchToSong(Song song)
		{
			CurrentSongIndex = GetSongIndex(song);
			OnPlaylistChanged();
		}

		private int GetSongIndex(Song song)
		{
			var songIndexes = SongItems.Select((item, i) => new { item.Song, Index = i })
				.Where(obj => obj.Song == song)
				.Select(obj => obj.Index)
				.ToList();

			if (!songIndexes.Any())
			{
				throw new InvalidOperationException("No matched song in the list");
			}

			if (songIndexes.Count > 1)
			{
				throw new InvalidOperationException("Multiple matched songs in the list");
			}

			return songIndexes.Single();
		}

		private void OnAddingNextSongs(IReadOnlyCollection<Song> songs)
		{
			int insertIndex = CurrentSongIndex + 1 ?? 0;
			int firstSongIndex = insertIndex;
			InsertSongs(insertIndex, songs);

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}

			OnPlaylistChanged();
		}

		private void OnAddingLastSongs(IReadOnlyCollection<Song> songs)
		{
			int firstSongIndex = SongItems.Count;
			AddSongs(songs);

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}

			OnPlaylistChanged();
		}

		internal void PlayFromSong()
		{
			var selectedSongIndex = SongItems.IndexOf(SelectedSongItem);
			if (selectedSongIndex != -1)
			{
				CurrentSongIndex = selectedSongIndex;
				Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs(CurrentSong));
			}
		}

		internal void NavigateToSongDisc()
		{
			var song = SelectedSongItem?.Song ?? CurrentSong;
			if (song == null)
			{
				return;
			}

			Messenger.Default.Send(new NavigateLibraryExplorerToDiscEventArgs(song.Disc));
		}
	}
}
