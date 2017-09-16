using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		private readonly DiscLibrary discLibrary;

		private readonly IWindowService windowService;
		private readonly ILibraryContentUpdater libraryContentUpdater;

		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public IExplorerSongListViewModel ExplorerSongListViewModel { get; }

		public ISongPlaylistViewModel Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		private int selectedSongListIndex;
		public int SelectedSongListIndex
		{
			get { return selectedSongListIndex; }
			set { Set(ref selectedSongListIndex, value); }
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(DiscLibrary discLibrary, ILibraryExplorerViewModel libraryExplorerViewModel, IExplorerSongListViewModel explorerSongListViewModel,
			IMusicPlayerViewModel musicPlayerViewModel, ILoggerViewModel loggerViewModel, IWindowService windowService, ILibraryContentUpdater libraryContentUpdater)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (libraryExplorerViewModel == null)
			{
				throw new ArgumentNullException(nameof(libraryExplorerViewModel));
			}
			if (explorerSongListViewModel == null)
			{
				throw new ArgumentNullException(nameof(explorerSongListViewModel));
			}
			if (musicPlayerViewModel == null)
			{
				throw new ArgumentNullException(nameof(musicPlayerViewModel));
			}
			if (loggerViewModel == null)
			{
				throw new ArgumentNullException(nameof(loggerViewModel));
			}
			if (windowService == null)
			{
				throw new ArgumentNullException(nameof(windowService));
			}
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.discLibrary = discLibrary;
			LibraryExplorerViewModel = libraryExplorerViewModel;
			ExplorerSongListViewModel = explorerSongListViewModel;
			MusicPlayerViewModel = musicPlayerViewModel;
			LoggerViewModel = loggerViewModel;
			this.windowService = windowService;
			this.libraryContentUpdater = libraryContentUpdater;

			LoadCommand = new AsyncRelayCommand(Load);

			LibraryExplorerViewModel.PropertyChanged += OnLibraryExplorerFolderChanged;
			Messenger.Default.Register<PlayDiscEventArgs>(this, OnPlayDiscLaunched);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<ReversePlayingEventArgs>(this, OnReversePlaying);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, async e => await OnPlaylistFinished(e));
		}

		private async Task Load()
		{
			await discLibrary.Load();
			LibraryExplorerViewModel.Load();
		}

		private void OnLibraryExplorerFolderChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LibraryExplorerViewModel.SelectedItem))
			{
				var discItem = LibraryExplorerViewModel.SelectedItem as DiscExplorerItem;
				if (discItem != null)
				{
					ExplorerSongListViewModel.SetSongs(discItem.Disc.Songs);
				}
				SwitchToExplorerSongList();
			}
		}

		private void OnPlayDiscLaunched(PlayDiscEventArgs message)
		{
			var disc = message.Disc;
			Playlist.SetSongs(disc.Songs);
			Playlist.SwitchToNextSong();
			MusicPlayerViewModel.Play();
			SwitchToSongPlaylist();
		}

		private void OnPlayDiscFromSongLaunched(PlayDiscFromSongEventArgs message)
		{
			var disc = message.Song.Disc;
			Playlist.SetSongs(disc.Songs);
			Playlist.SwitchToSong(message.Song);
			MusicPlayerViewModel.Play();
			SwitchToSongPlaylist();
		}

		private void OnReversePlaying(ReversePlayingEventArgs message)
		{
			if (MusicPlayerViewModel.IsPlaying)
			{
				MusicPlayerViewModel.Pause();
			}
			else
			{
				MusicPlayerViewModel.Resume();
			}
		}

		private async Task OnPlaylistFinished(PlaylistFinishedEventArgs e)
		{
			windowService.BringApplicationToFront();

			var playedDiscs = Playlist.Songs.Select(s => s.Disc).Distinct().ToList();
			if (playedDiscs.Count == 1)
			{
				var disc = playedDiscs.Single();
				var unratedSongsNumber = disc.Songs.Count(s => s.Rating == null);
				if (unratedSongsNumber > 0)
				{
					var rateDiscViewModel = new RateDiscViewModel(disc);
					if (unratedSongsNumber == disc.Songs.Count)
					{
						windowService.ShowRateDiscViewDialog(rateDiscViewModel);
						if (rateDiscViewModel.SelectedRating.HasValue)
						{
							await libraryContentUpdater.SetSongsRating(disc.Songs, rateDiscViewModel.SelectedRating.Value);
						}
					}
					else
					{
						windowService.ShowRateReminderViewDialog(rateDiscViewModel);
					}
				}
			}
		}

		private void SwitchToExplorerSongList()
		{
			SelectedSongListIndex = 0;
		}

		private void SwitchToSongPlaylist()
		{
			SelectedSongListIndex = 1;
		}
	}
}
