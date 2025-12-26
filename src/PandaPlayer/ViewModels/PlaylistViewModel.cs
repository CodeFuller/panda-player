using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Extensions;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels
{
	public class PlaylistViewModel : SongListViewModel, IPlaylistViewModel
	{
		private readonly IMessenger messenger;

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

		public override IEnumerable<BasicMenuItem> ContextMenuItems
		{
			get
			{
				var selectedSongItem = SelectedSongItem;
				var selectedSong = selectedSongItem?.Song;

				var selectedSongItems = (SelectedSongItems?.OfType<SongListItem>() ?? Enumerable.Empty<SongListItem>()).ToList();
				var selectedSongs = selectedSongItems.Select(x => x.Song).ToList();

				if (selectedSongItem != null)
				{
					yield return new CommandMenuItem(() => PlayFromSong(selectedSongItem, CancellationToken.None))
					{
						Header = "Play From This Song",
						IconKind = PackIconKind.Play,
					};
				}

				if (selectedSongItems.Count > 0)
				{
					yield return new CommandMenuItem(() => AddSongsNext(SelectedSongs.ToList(), CancellationToken.None))
					{
						Header = "Play Next",
						IconKind = PackIconKind.PlaylistAdd,
					};

					yield return new CommandMenuItem(() => AddSongsLast(SelectedSongs.ToList(), CancellationToken.None))
					{
						Header = "Play Last",
						IconKind = PackIconKind.PlaylistAdd,
					};

					yield return new CommandMenuItem(() => RemoveSongsFromPlaylist(selectedSongItems, CancellationToken.None))
					{
						Header = "Remove From Playlist",
						IconKind = PackIconKind.PlaylistMinus,
					};
				}

				if (SongItems.Any())
				{
					yield return new CommandMenuItem(() => ClearPlaylist(CancellationToken.None))
					{
						Header = "Clear Playlist",
						IconKind = PackIconKind.PlaylistRemove,
					};
				}

				if (selectedSong != null)
				{
					yield return new CommandMenuItem(() => messenger.Send(new NavigateLibraryExplorerToDiscEventArgs(selectedSong.Disc)))
					{
						Header = "Go To Disc",
						IconKind = PackIconKind.Album,
					};
				}

				if (selectedSongs.Count > 0)
				{
					yield return GetSetRatingContextMenuItem(selectedSongs);

					yield return new CommandMenuItem(() => EditSongsProperties(selectedSongs, CancellationToken.None))
					{
						Header = "Properties",
						IconKind = PackIconKind.Pencil,
					};
				}
			}
		}

		public PlaylistViewModel(ISongsService songsService, IViewNavigator viewNavigator, IMessenger messenger)
			: base(songsService, viewNavigator)
		{
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

			// There are 2 use cases of adding songs (Play Next & Play Last) to PlaylistViewModel:
			//   1. Action is invoked from context menu in DiscSongListViewModel.
			//      In this case DiscSongListViewModel sends AddingSongsToPlaylistNextEventArgs or AddingSongsToPlaylistLastEventArgs.
			//      Songs are added to PlaylistViewModel from handlers of these events.
			//
			//   2. Action is invoked from context menu in PlaylistViewModel.
			//      In ths case songs are added to PlaylistViewModel directly by calling AddSongsNext() & AddSongsLast() methods.
			//
			//   This is done to prevent anti-pattern when object sends event to itself.
			messenger.Register<AddingSongsToPlaylistNextEventArgs>(this, (_, e) => OnAddingNextSongs(e.Songs, CancellationToken.None));
			messenger.Register<AddingSongsToPlaylistLastEventArgs>(this, (_, e) => OnAddingLastSongs(e.Songs, CancellationToken.None));
		}

		protected void SetCurrentSong(int? songIndex)
		{
			CurrentSongIndex = songIndex;
			CurrentItem = songIndex == null ? null : SongItems[songIndex.Value];
		}

		protected virtual Task OnPlaylistChanged(CancellationToken cancellationToken)
		{
			messenger.Send(new PlaylistChangedEventArgs(Songs, CurrentSong, CurrentSongIndex));

			return Task.CompletedTask;
		}

		public async Task SetPlaylistSongs(IEnumerable<SongModel> songs, CancellationToken cancellationToken)
		{
			var songList = songs.ToList();

			SetSongs(songList);
			SetCurrentSong(songList.Count > 0 ? 0 : null);

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

		internal async Task AddSongsNext(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await InsertSongs(songs, CurrentSongIndex + 1 ?? 0, cancellationToken);
		}

		private async void OnAddingLastSongs(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await AddSongsLast(songs, cancellationToken);
		}

		internal async Task AddSongsLast(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			await InsertSongs(songs, SongItems.Count, cancellationToken);
		}

		private async Task InsertSongs(IReadOnlyCollection<SongModel> songs, int insertIndex, CancellationToken cancellationToken)
		{
			if (songs.Count == 0)
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

		internal async Task PlayFromSong(SongListItem songItem, CancellationToken cancellationToken)
		{
			var selectedSongIndex = SongItems.IndexOf(songItem);
			if (selectedSongIndex == -1)
			{
				return;
			}

			SetCurrentSong(selectedSongIndex);

			await OnPlaylistChanged(cancellationToken);

			messenger.Send(new PlayPlaylistStartingFromSongEventArgs());
		}

		internal async Task RemoveSongsFromPlaylist(IReadOnlyCollection<SongListItem> songItems, CancellationToken cancellationToken)
		{
			RemoveSongItems(songItems);

			var newCurrentItemIndex = SongItems.IndexOf(CurrentItem);
			SetCurrentSong(newCurrentItemIndex == -1 ? null : newCurrentItemIndex);

			await OnPlaylistChanged(cancellationToken);
		}

		internal async Task ClearPlaylist(CancellationToken cancellationToken)
		{
			RemoveSongItems(SongItems.ToList());

			SetCurrentSong(null);

			await OnPlaylistChanged(cancellationToken);
		}
	}
}
