using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.MenuItems;

namespace PandaPlayer.ViewModels
{
	public class DiscSongListViewModel : SongListViewModel, IDiscSongListViewModel
	{
		private readonly IMessenger messenger;

		private DiscModel CurrentDisc { get; set; }

		private bool DeletedContentIsShown { get; set; }

		public override bool DisplayTrackNumbers => true;

		public override IEnumerable<BasicMenuItem> ContextMenuItems
		{
			get
			{
				var selectedSongs = SelectedSongs.ToList();
				if (selectedSongs.Count == 0)
				{
					return Enumerable.Empty<BasicMenuItem>();
				}

				if (selectedSongs.All(x => !x.IsDeleted))
				{
					return GetContextMenuItemsForActiveSongs(selectedSongs);
				}

				if (selectedSongs.All(x => x.IsDeleted))
				{
					return GetContextMenuItemsForDeletedSongs(selectedSongs);
				}

				return Enumerable.Empty<BasicMenuItem>();
			}
		}

		public DiscSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator, IMessenger messenger)
			: base(songsService, viewNavigator)
		{
			this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
			messenger.Register<LibraryExplorerDiscChangedEventArgs>(this, (_, e) => OnExplorerDiscChanged(e.Disc, e.DeletedContentIsShown));
		}

		internal void DeleteSongsFromDisc(IReadOnlyCollection<SongModel> songs)
		{
			if (ViewNavigator.ShowDeleteDiscSongsView(songs))
			{
				LoadSongs();
			}
		}

		internal async Task UpdateSongsContent(IReadOnlyCollection<SongModel> songs, CancellationToken cancellationToken)
		{
			foreach (var song in songs)
			{
				await SongsService.UpdateSongContent(song, cancellationToken);
			}
		}

		private void OnExplorerDiscChanged(DiscModel newDisc, bool deletedContentIsShown)
		{
			CurrentDisc = newDisc;
			DeletedContentIsShown = deletedContentIsShown;

			LoadSongs();
		}

		private void LoadSongs()
		{
			IEnumerable<SongModel> songs;
			if (CurrentDisc == null)
			{
				songs = Enumerable.Empty<SongModel>();
			}
			else if (DeletedContentIsShown)
			{
				songs = CurrentDisc.AllSongsSorted;
			}
			else
			{
				songs = CurrentDisc.ActiveSongs;
			}

			SetSongs(songs);
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForActiveSongs(IReadOnlyCollection<SongModel> songs)
		{
			yield return new CommandMenuItem(() => messenger.Send(new AddingSongsToPlaylistNextEventArgs(songs)))
			{
				Header = "Play Next",
				IconKind = PackIconKind.PlaylistAdd,
			};

			yield return new CommandMenuItem(() => messenger.Send(new AddingSongsToPlaylistLastEventArgs(songs)))
			{
				Header = "Play Last",
				IconKind = PackIconKind.PlaylistAdd,
			};

			yield return GetSetRatingContextMenuItem(songs);

			yield return new CommandMenuItem(() => UpdateSongsContent(songs, CancellationToken.None))
			{
				Header = "Update Content",
				IconKind = PackIconKind.Refresh,
			};

			yield return new CommandMenuItem(() => DeleteSongsFromDisc(songs))
			{
				Header = "Delete From Disc",
				IconKind = PackIconKind.DeleteForever,
			};

			yield return new CommandMenuItem(() => EditSongsProperties(songs, CancellationToken.None))
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
			};
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForDeletedSongs(IReadOnlyCollection<SongModel> songs)
		{
			yield return new CommandMenuItem(() => EditSongsProperties(songs, CancellationToken.None))
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
			};
		}
	}
}
