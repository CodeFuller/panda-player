using System;
using System.Collections.Generic;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class SongPlaylistViewModel : SongListViewModel, ISongPlaylistViewModel
	{
		private int? currentSongIndex;
		public int? CurrentSongIndex
		{
			get { return currentSongIndex; }
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

				Messenger.Default.Send(new PlaylistChangedEventArgs(this));
			}
		}

		private SongListItem CurrentItem => SongItems != null && CurrentSongIndex != null ? SongItems[CurrentSongIndex.Value] : null;

		public override bool DisplayTrackNumbers => false;

		public Song CurrentSong => CurrentItem?.Song;

		public Disc PlayedDisc => Songs.Select(s => s.Disc).UniqueOrDefault();

		public SongPlaylistViewModel(ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
			: base(libraryContentUpdater, viewNavigator)
		{
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => OnAddingNextSongs(e.Songs));
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => OnAddingLastSongs(e.Songs));

			SongItems.CollectionChanged += (sender, e) => Messenger.Default.Send(new PlaylistChangedEventArgs(this));
		}

		public override void SetSongs(IEnumerable<Song> newSongs)
		{
			base.SetSongs(newSongs);
			CurrentSongIndex = null;
		}

		public void SwitchToNextSong()
		{
			CurrentSongIndex = CurrentSongIndex + 1 ?? 0;
		}

		public void SwitchToSong(Song song)
		{
			CurrentSongIndex = GetSongIndex(song);
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
			foreach (var song in songs)
			{
				SongItems.Insert(insertIndex++, new SongListItem(song));
			}

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}
		}

		private void OnAddingLastSongs(IReadOnlyCollection<Song> songs)
		{
			int firstSongIndex = SongItems.Count;
			foreach (var song in songs)
			{
				SongItems.Add(new SongListItem(song));
			}

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}
		}
	}
}
