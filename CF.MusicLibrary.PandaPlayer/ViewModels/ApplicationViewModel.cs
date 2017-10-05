using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		private const int ExplorerSongListIndex = 0;
		private const int PlaylistSongListIndex = 1;

		private readonly DiscLibrary discLibrary;

		private readonly IViewNavigator viewNavigator;

		public IApplicationViewModelHolder ViewModelHolder { get; }

		public ILibraryExplorerViewModel LibraryExplorerViewModel => ViewModelHolder.LibraryExplorerViewModel;

		public ISongPlaylistViewModel Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private int selectedSongListIndex;
		public int SelectedSongListIndex
		{
			get { return selectedSongListIndex; }
			set
			{
				Set(ref selectedSongListIndex, value);
				ActiveDisc = (SelectedSongListIndex == ExplorerSongListIndex) ? LibraryExplorerViewModel.SelectedDisc : PlaylistActiveDisc;
			}
		}

		private Disc activeDisc;
		private Disc ActiveDisc
		{
			set
			{
				if (activeDisc != value)
				{
					activeDisc = value;
					Messenger.Default.Send(new ActiveDiscChangedEventArgs(activeDisc));
				}
			}
		}

		private Disc PlaylistActiveDisc => Playlist.CurrentSong?.Disc ?? Playlist.PlayedDisc;

		public ICommand LoadCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ApplicationViewModel(DiscLibrary discLibrary, IApplicationViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (viewModelHolder == null)
			{
				throw new ArgumentNullException(nameof(viewModelHolder));
			}
			if (musicPlayerViewModel == null)
			{
				throw new ArgumentNullException(nameof(musicPlayerViewModel));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}

			this.discLibrary = discLibrary;
			ViewModelHolder = viewModelHolder;
			MusicPlayerViewModel = musicPlayerViewModel;
			this.viewNavigator = viewNavigator;

			LoadCommand = new AsyncRelayCommand(Load);
			ShowLibraryStatisticsCommand = new RelayCommand(ShowLibraryStatistics);

			Messenger.Default.Register<PlayDiscEventArgs>(this, OnPlayDiscLaunched);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, OnPlayPlaylistStartingFromSong);
			Messenger.Default.Register<ReversePlayingEventArgs>(this, OnReversePlaying);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => SwitchToExplorerSongList());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => OnPlaylistSongChanged());
		}

		public async Task Load()
		{
			await discLibrary.Load();
			Messenger.Default.Send(new LibraryLoadedEventArgs(discLibrary));

			//	Setting playlist active if some previous playlist was loaded.
			if (Playlist.Songs.Any())
			{
				SwitchToSongPlaylist();
			}
		}

		public void ShowLibraryStatistics()
		{
			viewNavigator.ShowLibraryStatisticsView();
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

		private void OnPlayPlaylistStartingFromSong(PlayPlaylistStartingFromSongEventArgs message)
		{
			MusicPlayerViewModel.PlayFromSong(message.Song);
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
				MusicPlayerViewModel.Play();
			}
		}

		private void OnPlaylistSongChanged()
		{
			if (SelectedSongListIndex == PlaylistSongListIndex)
			{
				ActiveDisc = PlaylistActiveDisc;
			}
		}

		private void OnPlaylistFinished(PlaylistFinishedEventArgs e)
		{
			viewNavigator.BringApplicationToFront();

			var playedDisc = Playlist.PlayedDisc;
			if (playedDisc == null || playedDisc.Songs.All(s => s.Rating != null))
			{
				return;
			}

			viewNavigator.ShowRateDiscView(playedDisc);
		}

		private void SwitchToExplorerSongList()
		{
			SelectedSongListIndex = ExplorerSongListIndex;
		}

		private void SwitchToSongPlaylist()
		{
			SelectedSongListIndex = PlaylistSongListIndex;
		}
	}
}
