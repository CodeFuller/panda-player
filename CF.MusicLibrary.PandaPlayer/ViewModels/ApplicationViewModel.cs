using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		private readonly DiscLibrary discLibrary;

		private readonly IViewNavigator viewNavigator;

		public IViewModelHolder ViewModelHolder { get; }

		public ILibraryExplorerViewModel LibraryExplorerViewModel => ViewModelHolder.LibraryExplorerViewModel;

		public ISongPlaylistViewModel Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private int selectedSongListIndex;
		public int SelectedSongListIndex
		{
			get { return selectedSongListIndex; }
			set { Set(ref selectedSongListIndex, value); }
		}

		public ICommand LoadCommand { get; }

		public ApplicationViewModel(DiscLibrary discLibrary, IViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
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

			Messenger.Default.Register<PlayDiscEventArgs>(this, OnPlayDiscLaunched);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<ReversePlayingEventArgs>(this, OnReversePlaying);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => SwitchToExplorerSongList());
		}

		private async Task Load()
		{
			await discLibrary.Load();
			Messenger.Default.Send(new LibraryLoadedEventArgs());
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
			SelectedSongListIndex = 0;
		}

		private void SwitchToSongPlaylist()
		{
			SelectedSongListIndex = 1;
		}
	}
}
