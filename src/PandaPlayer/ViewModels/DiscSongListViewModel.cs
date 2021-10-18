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

		public IEnumerable<BasicMenuItem> ContextMenuItems
		{
			get
			{
				var selectedSongs = SelectedSongs.ToList();
				if (!selectedSongs.Any())
				{
					yield break;
				}

				if (selectedSongs.Any(x => x.IsDeleted))
				{
					yield break;
				}

				yield return new CommandMenuItem
				{
					Header = "Play Next",
					IconKind = PackIconKind.PlaylistAdd,
					Command = new RelayCommand(() => Messenger.Default.Send(new AddingSongsToPlaylistNextEventArgs(selectedSongs))),
				};

				yield return new CommandMenuItem
				{
					Header = "Play Last",
					IconKind = PackIconKind.PlaylistAdd,
					Command = new RelayCommand(() => Messenger.Default.Send(new AddingSongsToPlaylistLastEventArgs(selectedSongs))),
				};

				yield return GetSetRatingContextMenuItem(selectedSongs);

				yield return new CommandMenuItem
				{
					Header = "Delete From Disc",
					IconKind = PackIconKind.DeleteForever,
					Command = new RelayCommand(DeleteSongsFromDisc),
				};

				yield return new CommandMenuItem
				{
					Header = "Properties",
					IconKind = PackIconKind.Pencil,
					Command = new AsyncRelayCommand(() => EditSongsProperties(selectedSongs, CancellationToken.None)),
				};
			}
		}

		public DiscSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
			: base(songsService, viewNavigator)
		{
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => OnExplorerDiscChanged(e.Disc, e.DeletedContentIsShown));
		}

		private void DeleteSongsFromDisc()
		{
			var selectedSongs = SelectedSongs.ToList();
			if (!selectedSongs.Any())
			{
				return;
			}

			ViewNavigator.ShowDeleteDiscSongsView(selectedSongs);

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
	}
}
