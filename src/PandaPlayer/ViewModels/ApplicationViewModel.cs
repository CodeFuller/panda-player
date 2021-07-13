using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events;
using PandaPlayer.Events.DiscEvents;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels
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

		public ISongListViewModel DiscSongListViewModel => LibraryExplorerViewModel.DiscSongListViewModel;

		public IPlaylistViewModel PlaylistViewModel => MusicPlayerViewModel.Playlist;

		private ISongListViewModel currentSongListViewModel;

		private ISongListViewModel CurrentSongListViewModel
		{
			get => currentSongListViewModel;
			set
			{
				Set(ref currentSongListViewModel, value);
				RaisePropertyChanged(nameof(IsDiscSongListSelected));
				RaisePropertyChanged(nameof(IsPlaylistSelected));

				ActiveDisc = IsDiscSongListSelected ? LibraryExplorerViewModel.SelectedDisc : PlaylistActiveDisc;
			}
		}

		public bool IsDiscSongListSelected => CurrentSongListViewModel == DiscSongListViewModel;

		public bool IsPlaylistSelected => CurrentSongListViewModel == PlaylistViewModel;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private DiscModel activeDisc;

		private DiscModel ActiveDisc
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

		private DiscModel PlaylistActiveDisc => PlaylistViewModel.CurrentDisc;

		public ICommand LoadCommand { get; }

		public ICommand ReversePlayingCommand { get; }

		public ICommand ShowDiscAdderCommand { get; }

		public ICommand ShowLibraryCheckerCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ICommand SwitchToDiscSongListCommand { get; }

		public ICommand SwitchToPlaylistCommand { get; }

		public ApplicationViewModel(IApplicationViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
		{
			ViewModelHolder = viewModelHolder ?? throw new ArgumentNullException(nameof(viewModelHolder));
			MusicPlayerViewModel = musicPlayerViewModel ?? throw new ArgumentNullException(nameof(musicPlayerViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			LoadCommand = new RelayCommand(Load);
			ReversePlayingCommand = new AsyncRelayCommand(() => ReversePlaying(CancellationToken.None));
			ShowDiscAdderCommand = new AsyncRelayCommand(() => ShowDiscAdder(CancellationToken.None));
			ShowLibraryCheckerCommand = new AsyncRelayCommand(() => ShowLibraryChecker(CancellationToken.None));
			ShowLibraryStatisticsCommand = new AsyncRelayCommand(() => ShowLibraryStatistics(CancellationToken.None));

			SwitchToDiscSongListCommand = new RelayCommand(SwitchToDiscSongList);
			SwitchToPlaylistCommand = new RelayCommand(SwitchToPlaylist);

			CurrentSongListViewModel = DiscSongListViewModel;

			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaySongList(e, CancellationToken.None));
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, _ => OnPlayPlaylistStartingFromSong(CancellationToken.None));
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, _ => SwitchToDiscSongList());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, OnPlaylistSongChanged);
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, OnPlaylistSongChanged);
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => NavigateLibraryExplorerToDisc(e.Disc, CancellationToken.None));
		}

		public void Load()
		{
			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Setting playlist active if some previous playlist was loaded.
			if (PlaylistViewModel.Songs.Any())
			{
				SwitchToPlaylist();
			}
		}

		public async Task ShowDiscAdder(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowDiscAdderView(cancellationToken);
		}

		public async Task ShowLibraryChecker(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowLibraryCheckerView(cancellationToken);
		}

		public async Task ShowLibraryStatistics(CancellationToken cancellationToken)
		{
			await viewNavigator.ShowLibraryStatisticsView(cancellationToken);
		}

		private async void OnPlaySongList(PlaySongsListEventArgs e, CancellationToken cancellationToken)
		{
			await PlaylistViewModel.SetPlaylistSongs(e.Songs, cancellationToken);
			await PlaylistViewModel.SwitchToNextSong(cancellationToken);
			await ResetPlayer(cancellationToken);
			SwitchToPlaylist();
		}

		private async void OnPlayPlaylistStartingFromSong(CancellationToken cancellationToken)
		{
			await ResetPlayer(cancellationToken);
			SwitchToPlaylist();
		}

		internal async Task ReversePlaying(CancellationToken cancellationToken)
		{
			await MusicPlayerViewModel.ReversePlaying(cancellationToken);
		}

		private async Task ResetPlayer(CancellationToken cancellationToken)
		{
			MusicPlayerViewModel.Stop();
			await MusicPlayerViewModel.Play(cancellationToken);
		}

		private void OnPlaylistSongChanged(BasicPlaylistEventArgs e)
		{
			Title = e.CurrentSong != null ? BuildCurrentTitle(e.CurrentSong, e.CurrentSongIndex) : DefaultTitle;

			if (IsPlaylistSelected)
			{
				ActiveDisc = PlaylistActiveDisc;
			}
		}

		private string BuildCurrentTitle(SongModel song, int? playlistSongIndex)
		{
			var songTitle = song.Artist != null ? $"{song.Artist.Name} - {song.Title}" : song.Title;
			return $"{playlistSongIndex + 1}/{PlaylistViewModel.SongsNumber} - {songTitle}";
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

		private void SwitchToDiscSongList()
		{
			CurrentSongListViewModel = DiscSongListViewModel;
		}

		private void SwitchToPlaylist()
		{
			CurrentSongListViewModel = PlaylistViewModel;
		}

		private async void NavigateLibraryExplorerToDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await LibraryExplorerViewModel.SwitchToDisc(disc, cancellationToken);
			SwitchToDiscSongList();
		}
	}
}
