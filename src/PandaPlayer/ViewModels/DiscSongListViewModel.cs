using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
{
	public class DiscSongListViewModel : SongListViewModel, IDiscSongListViewModel
	{
		public override bool DisplayTrackNumbers => true;

		public override ICommand PlaySongsNextCommand { get; }

		public override ICommand PlaySongsLastCommand { get; }

		public ICommand DeleteSongsFromDiscCommand { get; }

		public DiscSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator)
			: base(songsService, viewNavigator)
		{
			PlaySongsNextCommand = new RelayCommand(() => SendAddingSongsToPlaylistEvent(new AddingSongsToPlaylistNextEventArgs(SelectedSongs)));
			PlaySongsLastCommand = new RelayCommand(() => SendAddingSongsToPlaylistEvent(new AddingSongsToPlaylistLastEventArgs(SelectedSongs)));
			DeleteSongsFromDiscCommand = new RelayCommand(DeleteSongsFromDisc);

			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => OnExplorerDiscChanged(e.Disc, e.DeletedContentIsShown));
		}

		private static void SendAddingSongsToPlaylistEvent<TAddingSongsToPlaylistEventArgs>(TAddingSongsToPlaylistEventArgs e)
			where TAddingSongsToPlaylistEventArgs : AddingSongsToPlaylistEventArgs
		{
			if (e.Songs.Any())
			{
				Messenger.Default.Send(e);
			}
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
				songs = newDisc.AllSongs;
			}
			else
			{
				songs = newDisc.ActiveSongs;
			}

			SetSongs(songs);
		}
	}
}
