using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using CodeFuller.Library.Wpf.Interfaces;
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
		private readonly IWindowService windowService;

		public override bool DisplayTrackNumbers => true;

		public override ICommand PlaySongsNextCommand { get; }

		public override ICommand PlaySongsLastCommand { get; }

		public ICommand DeleteSongsFromDiscCommand { get; }

		public DiscSongListViewModel(ISongsService songsService, IViewNavigator viewNavigator, IWindowService windowService)
			: base(songsService, viewNavigator)
		{
			this.windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));

			PlaySongsNextCommand = new RelayCommand(() => SendAddingSongsToPlaylistEvent(new AddingSongsToPlaylistNextEventArgs(SelectedSongs)));
			PlaySongsLastCommand = new RelayCommand(() => SendAddingSongsToPlaylistEvent(new AddingSongsToPlaylistLastEventArgs(SelectedSongs)));
			DeleteSongsFromDiscCommand = new AsyncRelayCommand(() => DeleteSongsFromDisc(CancellationToken.None));

			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => OnExplorerDiscChanged(e.Disc));
		}

		private static void SendAddingSongsToPlaylistEvent<TAddingSongsToPlaylistEventArgs>(TAddingSongsToPlaylistEventArgs e)
			where TAddingSongsToPlaylistEventArgs : AddingSongsToPlaylistEventArgs
		{
			if (e.Songs.Any())
			{
				Messenger.Default.Send(e);
			}
		}

		private async Task DeleteSongsFromDisc(CancellationToken cancellationToken)
		{
			var selectedSongs = SelectedSongs.ToList();
			if (!selectedSongs.Any())
			{
				return;
			}

			if (windowService.ShowMessageBox($"Do you really want to delete {selectedSongs.Count} selected song(s)?", "Delete song(s)",
				ShowMessageBoxButton.YesNo, ShowMessageBoxIcon.Warning) != ShowMessageBoxResult.Yes)
			{
				return;
			}

			foreach (var song in selectedSongs)
			{
				await SongsService.DeleteSong(song, cancellationToken);
			}

			// Above call to songsService.DeleteSong() updates song.DeleteDate.
			// As result OnSongChanged() is called, which deletes song(s) from the list.
		}

		private void OnExplorerDiscChanged(DiscModel newDisc)
		{
			SetSongs(newDisc?.ActiveSongs ?? Enumerable.Empty<SongModel>());
		}
	}
}
