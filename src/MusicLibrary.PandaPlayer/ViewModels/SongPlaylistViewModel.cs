using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Comparers;
using MusicLibrary.Core.Extensions;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.Services.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels
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

		public SongModel CurrentSong => CurrentItem?.Song;

		public DiscModel PlayingDisc => Songs.UniqueOrDefault(new SongEqualityComparer())?.Disc;

		public override ICommand PlayFromSongCommand { get; }

		public ICommand NavigateToSongDiscCommand { get; }

		public SongPlaylistViewModel(ISongsService songsService, IViewNavigator viewNavigator, IWindowService windowService)
			: base(songsService, viewNavigator, windowService)
		{
			PlayFromSongCommand = new RelayCommand(PlayFromSong);
			NavigateToSongDiscCommand = new RelayCommand(NavigateToSongDisc);

			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => OnAddingNextSongs(e.Songs, CancellationToken.None));
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => OnAddingLastSongs(e.Songs, CancellationToken.None));
		}

		protected virtual Task OnPlaylistChanged(CancellationToken cancellationToken)
		{
			Messenger.Default.Send(new PlaylistChangedEventArgs(this));

			return Task.CompletedTask;
		}

		public async Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			CurrentSongIndex = null;
			SetSongsRaw(songs);
			await OnPlaylistChanged(cancellationToken);
		}

		protected void SetSongsRaw(IEnumerable<SongModel> newSongs)
		{
			SetSongs(newSongs);
		}

		public async Task SwitchToNextSong(CancellationToken cancellationToken)
		{
			CurrentSongIndex = CurrentSongIndex + 1 ?? 0;
			await OnPlaylistChanged(cancellationToken);
		}

		public async Task SwitchToSong(SongModel song, CancellationToken cancellationToken)
		{
			CurrentSongIndex = GetSongIndex(song);
			await OnPlaylistChanged(cancellationToken);
		}

		private int GetSongIndex(SongModel song)
		{
			var songIndexes = SongItems.Select((item, i) => new { item.Song, Index = i })
				.Where(obj => obj.Song.Id == song.Id)
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

		private async void OnAddingNextSongs(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			var insertIndex = CurrentSongIndex + 1 ?? 0;
			var firstSongIndex = insertIndex;
			InsertSongs(insertIndex, songs);

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}

			await OnPlaylistChanged(cancellationToken);
		}

		private async void OnAddingLastSongs(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			var firstSongIndex = SongItems.Count;
			AddSongs(songs);

			if (CurrentItem == null && songs.Any())
			{
				CurrentSongIndex = firstSongIndex;
			}

			await OnPlaylistChanged(cancellationToken);
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
