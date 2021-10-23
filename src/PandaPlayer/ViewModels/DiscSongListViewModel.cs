using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
		public override bool DisplayTrackNumbers => true;

		public override IEnumerable<BasicMenuItem> ContextMenuItems
		{
			get
			{
				var selectedSongs = SelectedSongs.ToList();
				if (!selectedSongs.Any())
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

		public DiscSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
			: base(songsService, viewNavigator)
		{
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => OnExplorerDiscChanged(e.Disc, e.DeletedContentIsShown));
		}

		private void DeleteSongsFromDisc(IReadOnlyCollection<SongModel> songs)
		{
			ViewNavigator.ShowDeleteDiscSongsView(songs);

			// If songs are deleted, call to songsService.DeleteSong() updates song.DeleteDate.
			// As result OnSongChanged() is called, which deletes song(s) from the list.
		}

		private void OnExplorerDiscChanged(DiscModel newDisc, bool deletedContentIsShown)
		{
			IEnumerable<SongModel> songs;
			if (newDisc == null)
			{
				songs = Enumerable.Empty<SongModel>();
			}
			else if (deletedContentIsShown)
			{
				songs = newDisc.AllSongsSorted;
			}
			else
			{
				songs = newDisc.ActiveSongs;
			}

			SetSongs(songs);
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForActiveSongs(IReadOnlyCollection<SongModel> songs)
		{
			yield return new CommandMenuItem
			{
				Header = "Play Next",
				IconKind = PackIconKind.PlaylistAdd,
				Command = new RelayCommand(() => Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(songs)), keepTargetAlive: true),
			};

			yield return new CommandMenuItem
			{
				Header = "Play Last",
				IconKind = PackIconKind.PlaylistAdd,
				Command = new RelayCommand(() => Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(songs)), keepTargetAlive: true),
			};

			yield return GetSetRatingContextMenuItem(songs);

			yield return new CommandMenuItem
			{
				Header = "Delete From Disc",
				IconKind = PackIconKind.DeleteForever,
				Command = new RelayCommand(() => DeleteSongsFromDisc(songs), keepTargetAlive: true),
			};

			yield return new CommandMenuItem
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
				Command = new AsyncRelayCommand(() => EditSongsProperties(songs, CancellationToken.None)),
			};
		}

		private IEnumerable<BasicMenuItem> GetContextMenuItemsForDeletedSongs(IReadOnlyCollection<SongModel> songs)
		{
			yield return new CommandMenuItem
			{
				Header = "Properties",
				IconKind = PackIconKind.Pencil,
				Command = new AsyncRelayCommand(() => EditSongsProperties(songs, CancellationToken.None)),
			};
		}
	}
}
