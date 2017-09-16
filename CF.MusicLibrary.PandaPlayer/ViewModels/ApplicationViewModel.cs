using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
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
			IMusicPlayerViewModel musicPlayerViewModel, ILoggerViewModel loggerViewModel)
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

			this.discLibrary = discLibrary;
			LibraryExplorerViewModel = libraryExplorerViewModel;
			ExplorerSongListViewModel = explorerSongListViewModel;
			MusicPlayerViewModel = musicPlayerViewModel;
			LoggerViewModel = loggerViewModel;

			LoadCommand = new AsyncRelayCommand(Load);

			LibraryExplorerViewModel.PropertyChanged += OnLibraryExplorerFolderChanged;
			Messenger.Default.Register<PlayDiscEventArgs>(this, OnPlayDiscLaunched);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<ReversePlayingEventArgs>(this, OnReversePlaying);
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
