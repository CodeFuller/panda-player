using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	internal class ApplicationViewModel : ViewModelBase
	{
		private const string DefaultTitle = "Panda Player";

		private readonly IViewNavigator viewNavigator;

		private string title = DefaultTitle;

		public string Title
		{
			get => title;
			set => Set(ref title, value);
		}

		public IApplicationViewModelHolder ViewModelHolder { get; }

		public ILibraryExplorerViewModel LibraryExplorerViewModel => ViewModelHolder.LibraryExplorerViewModel;

		public ISongListViewModel ExplorerSongsViewModel => LibraryExplorerViewModel.SongListViewModel;

		public ISongPlaylistViewModel PlaylistSongsViewModel => MusicPlayerViewModel.Playlist;

		private ISongListViewModel currentSongListViewModel;

		private ISongListViewModel CurrentSongListViewModel
		{
			get => currentSongListViewModel;
			set
			{
				Set(ref currentSongListViewModel, value);
				RaisePropertyChanged(nameof(AreExplorerSongsSelected));
				RaisePropertyChanged(nameof(ArePlaylistSongsSelected));

				ActiveDisc = AreExplorerSongsSelected ? LibraryExplorerViewModel.SelectedDisc : PlaylistActiveDisc;
			}
		}

		public bool AreExplorerSongsSelected => CurrentSongListViewModel == ExplorerSongsViewModel;

		public bool ArePlaylistSongsSelected => CurrentSongListViewModel == PlaylistSongsViewModel;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private DiscModel activeDisc;

		private DiscModel ActiveDisc
		{
			set
			{
				// TBD: Revise objects comparing
				if (activeDisc != value)
				{
					activeDisc = value;
					Messenger.Default.Send(new ActiveDiscChangedEventArgs(activeDisc));
				}
			}
		}

		private DiscModel PlaylistActiveDisc => PlaylistSongsViewModel.CurrentSong?.Disc ?? PlaylistSongsViewModel.PlayingDisc;

		public ICommand LoadCommand { get; }

		public ICommand ReversePlayingCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ICommand SwitchToExplorerSongsCommand { get; }

		public ICommand SwitchToPlaylistSongsCommand { get; }

		public ApplicationViewModel(IApplicationViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
		{
			ViewModelHolder = viewModelHolder ?? throw new ArgumentNullException(nameof(viewModelHolder));
			MusicPlayerViewModel = musicPlayerViewModel ?? throw new ArgumentNullException(nameof(musicPlayerViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			LoadCommand = new RelayCommand(Load);
			ReversePlayingCommand = new RelayCommand(ReversePlaying);
			ShowLibraryStatisticsCommand = new AsyncRelayCommand(ShowLibraryStatistics);

			SwitchToExplorerSongsCommand = new RelayCommand(SwitchToExplorerSongs);
			SwitchToPlaylistSongsCommand = new RelayCommand(SwitchToPlaylistSongs);

			CurrentSongListViewModel = ExplorerSongsViewModel;

			Messenger.Default.Register<PlaySongsListEventArgs>(this, OnPlaySongList);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, OnPlayPlaylistStartingFromSong);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => SwitchToExplorerSongs());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => NavigateLibraryExplorerToDisc(e.Disc));
		}

		public void Load()
		{
			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Setting playlist active if some previous playlist was loaded.
			if (PlaylistSongsViewModel.Songs.Any())
			{
				SwitchToPlaylistSongs();
			}
		}

		public async Task ShowLibraryStatistics()
		{
			await viewNavigator.ShowLibraryStatisticsView(CancellationToken.None);
		}

		private void OnPlaySongList(PlaySongsListEventArgs e)
		{
			PlaylistSongsViewModel.SetSongs(e.Songs);
			PlaylistSongsViewModel.SwitchToNextSong();
			ResetPlayer();
			SwitchToPlaylistSongs();
		}

		private void OnPlayDiscFromSongLaunched(PlayDiscFromSongEventArgs message)
		{
			var disc = message.Song.Disc;

			PlaylistSongsViewModel.SetSongs(disc.ActiveSongs);
			PlaylistSongsViewModel.SwitchToSong(message.Song);
			ResetPlayer();
			SwitchToPlaylistSongs();
		}

		private void OnPlayPlaylistStartingFromSong(PlayPlaylistStartingFromSongEventArgs message)
		{
			ResetPlayer();
			SwitchToPlaylistSongs();
		}

		internal void ReversePlaying()
		{
			// TODO: ReversePlaying is an async method, which must be awaited.
			MusicPlayerViewModel.ReversePlaying();
		}

		private void ResetPlayer()
		{
			// TODO: Play is an async method, which must be awaited.
			MusicPlayerViewModel.Stop();
			MusicPlayerViewModel.Play();
		}

		private void OnPlaylistSongChanged()
		{
			Title = PlaylistSongsViewModel.CurrentSong != null ? BuildCurrentTitle(PlaylistSongsViewModel.CurrentSong) : DefaultTitle;

			if (ArePlaylistSongsSelected)
			{
				ActiveDisc = PlaylistActiveDisc;
			}
		}

		private string BuildCurrentTitle(SongModel song)
		{
			var songTitle = song.Artist != null ? Current($"{song.Artist.Name} - {song.Title}") : song.Title;
			return Current($"{PlaylistSongsViewModel.CurrentSongIndex + 1}/{PlaylistSongsViewModel.SongsNumber} - {songTitle}");
		}

		private void OnPlaylistFinished(PlaylistFinishedEventArgs e)
		{
			var songs = e.Songs;
			if (songs.All(s => s.Rating != null))
			{
				return;
			}

			viewNavigator.ShowRatePlaylistSongsView(songs);
		}

		private void SwitchToExplorerSongs()
		{
			CurrentSongListViewModel = ExplorerSongsViewModel;
		}

		private void SwitchToPlaylistSongs()
		{
			CurrentSongListViewModel = PlaylistSongsViewModel;
		}

		private void NavigateLibraryExplorerToDisc(DiscModel disc)
		{
			LibraryExplorerViewModel.SwitchToDisc(disc);
			SwitchToExplorerSongs();
		}
	}
}
