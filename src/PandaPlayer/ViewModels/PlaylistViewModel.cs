using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class PlaylistViewModel : SongListViewModel, IPlaylistViewModel
	{
		// CurrentSongIndex and CurrentItem are kept in-sync via method SetCurrentSong.
		// Neither CurrentSongIndex nor CurrentItem should be set directly.
		protected int? CurrentSongIndex { get; private set; }

		private SongListItem currentItem;

		private SongListItem CurrentItem
		{
			get => currentItem;
			set
			{
				if (ReferenceEquals(currentItem, value))
				{
					return;
				}

				if (currentItem != null)
				{
					currentItem.IsCurrentlyPlayed = false;
				}

				currentItem = value;

				if (currentItem != null)
				{
					currentItem.IsCurrentlyPlayed = true;
				}
			}
		}

		public override bool DisplayTrackNumbers => false;

		public SongModel CurrentSong => CurrentItem?.Song;

		public DiscModel CurrentDisc => CurrentSong != null ? CurrentSong.Disc : Songs.Select(s => s.Disc).UniqueOrDefault(new DiscEqualityComparer());

		public override ICommand PlaySongsNextCommand { get; }

		public override ICommand PlaySongsLastCommand { get; }

		public ICommand PlayFromSongCommand { get; }

		public ICommand RemoveSongsFromPlaylistCommand { get; }

		public ICommand ClearPlaylistCommand { get; }

		public ICommand NavigateToSongDiscCommand { get; }

		public PlaylistViewModel(ISongsService songsService, IViewNavigator viewNavigator)
			: base(songsService, viewNavigator)
		{
			PlayFromSongCommand = new AsyncRelayCommand(() => PlayFromSong(CancellationToken.None));
			RemoveSongsFromPlaylistCommand = new AsyncRelayCommand(() => RemoveSongsFromPlaylist(CancellationToken.None));
			ClearPlaylistCommand = new AsyncRelayCommand(() => ClearPlaylist(CancellationToken.None));
			NavigateToSongDiscCommand = new RelayCommand(NavigateToSongDisc);

			// There are 2 use cases of adding songs (Play Next & Play Last) to PlaylistViewModel:
			//   1. Action is invoked from context menu in DiscSongListViewModel.
			//      In this case DiscSongListViewModel sends AddingSongsToPlaylistNextEventArgs or AddingSongsToPlaylistLastEventArgs.
			//      Songs are added to PlaylistViewModel from handlers of these events.
			//
			//   2. Action is invoked from context menu in PlaylistViewModel.
			//      In ths case songs are added to PlaylistViewModel from handlers of PlaySongsNextCommand and PlaySongsLastCommand.
			//
			//   This is done to prevent anti-pattern when object sends event to itself.
			PlaySongsNextCommand = new AsyncRelayCommand(() => AddSongsNext(SelectedSongs.ToList(), CancellationToken.None));
			PlaySongsLastCommand = new AsyncRelayCommand(() => AddSongsLast(SelectedSongs.ToList(), CancellationToken.None));
			Messenger.Default.Register<AddingSongsToPlaylistNextEventArgs>(this, e => OnAddingNextSongs(e.Songs, CancellationToken.None));
			Messenger.Default.Register<AddingSongsToPlaylistLastEventArgs>(this, e => OnAddingLastSongs(e.Songs, CancellationToken.None));
		}

		protected void SetCurrentSong(int? songIndex)
		{
			CurrentSongIndex = songIndex;
			CurrentItem = songIndex == null ? null : SongItems[songIndex.Value];
		}

		protected virtual Task OnPlaylistChanged(CancellationToken cancellationToken)
		{
			Messenger.Default.Send(new PlaylistChangedEventArgs(Songs, CurrentSong, CurrentSongIndex));

			return Task.CompletedTask;
		}

		public async Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			var songList = songs.ToList();

			SetSongs(songList);
			SetCurrentSong(songList.Any() ? 0 : null);

			await OnPlaylistChanged(cancellationToken);
		}

		public async Task SwitchToNextSong(CancellationToken cancellationToken)
		{
			if (CurrentSong == null)
			{
				throw new InvalidOperationException("Current song is not set");
			}

			var newSongIndex = CurrentSongIndex + 1;
			if (newSongIndex >= SongItems.Count)
			{
				newSongIndex = null;
			}

			SetCurrentSong(newSongIndex);

			await OnPlaylistChanged(cancellationToken);
		}

		private async void OnAddingNextSongs(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await AddSongsNext(songs, cancellationToken);
		}

		private async Task AddSongsNext(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await InsertSongs(songs, CurrentSongIndex + 1 ?? 0, cancellationToken);
		}

		private async void OnAddingLastSongs(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await AddSongsLast(songs, cancellationToken);
		}

		private async Task AddSongsLast(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await InsertSongs(songs, SongItems.Count, cancellationToken);
		}

		private async Task InsertSongs(IReadOnlyCollection<SongModel> songs, int insertIndex, CancellationToken cancellationToken)
		{
			if (!songs.Any())
			{
				return;
			}

			InsertSongs(insertIndex, songs);

			if (CurrentItem == null)
			{
				SetCurrentSong(insertIndex);
			}

			await OnPlaylistChanged(cancellationToken);
		}

		private async Task PlayFromSong(CancellationToken cancellationToken)
		{
			var selectedSongIndex = SongItems.IndexOf(SelectedSongItem);
			if (selectedSongIndex == -1)
			{
				return;
			}

			SetCurrentSong(selectedSongIndex);

			await OnPlaylistChanged(cancellationToken);

			Messenger.Default.Send(new PlayPlaylistStartingFromSongEventArgs());
		}

		private async Task RemoveSongsFromPlaylist(CancellationToken cancellationToken)
		{
			var songsToDelete = (SelectedSongItems?.OfType<SongListItem>() ?? Enumerable.Empty<SongListItem>()).ToList();
			if (!songsToDelete.Any())
			{
				return;
			}

			RemoveSongItems(songsToDelete);

			var newCurrentItemIndex = SongItems.IndexOf(CurrentItem);
			SetCurrentSong(newCurrentItemIndex == -1 ? null : newCurrentItemIndex);

			await OnPlaylistChanged(cancellationToken);
		}

		private async Task ClearPlaylist(CancellationToken cancellationToken)
		{
			if (!SongItems.Any())
			{
				return;
			}

			RemoveSongItems(SongItems.ToList());

			SetCurrentSong(null);

			await OnPlaylistChanged(cancellationToken);
		}

		private void NavigateToSongDisc()
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
