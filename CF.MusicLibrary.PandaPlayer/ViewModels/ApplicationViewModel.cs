using System;
using System.ComponentModel;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.Player;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		public ILibraryExplorerViewModel LibraryExplorerViewModel { get; }

		public ISongListViewModel SongListViewModel { get; }

		public ISongPlaylist Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		public ILoggerViewModel LoggerViewModel { get; }

		private int selectedSongListIndex;
		public int SelectedSongListIndex
		{
			get { return selectedSongListIndex; }
			set { Set(ref selectedSongListIndex, value); }
		}

		public ApplicationViewModel(ILibraryExplorerViewModel libraryExplorerViewModel, ISongListViewModel songListViewModel,
			IMusicPlayerViewModel musicPlayerViewModel, ILoggerViewModel loggerViewModel)
		{
			if (libraryExplorerViewModel == null)
			{
				throw new ArgumentNullException(nameof(libraryExplorerViewModel));
			}
			if (songListViewModel == null)
			{
				throw new ArgumentNullException(nameof(songListViewModel));
			}
			if (musicPlayerViewModel == null)
			{
				throw new ArgumentNullException(nameof(musicPlayerViewModel));
			}
			if (loggerViewModel == null)
			{
				throw new ArgumentNullException(nameof(loggerViewModel));
			}

			LibraryExplorerViewModel = libraryExplorerViewModel;
			SongListViewModel = songListViewModel;
			MusicPlayerViewModel = musicPlayerViewModel;
			LoggerViewModel = loggerViewModel;

			LibraryExplorerViewModel.PropertyChanged += OnLibraryExplorerFolderChanged;
			Messenger.Default.Register<PlayDiscEventArgs>(this, OnPlayDiscLaunched);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<ReversePlayingEventArgs>(this, OnReversePlaying);
		}

		private void OnLibraryExplorerFolderChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LibraryExplorerViewModel.SelectedItem))
			{
				var discItem = LibraryExplorerViewModel.SelectedItem as DiscExplorerItem;
				if (discItem != null)
				{
					SongListViewModel.SetSongs(discItem.Disc.Songs);
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
