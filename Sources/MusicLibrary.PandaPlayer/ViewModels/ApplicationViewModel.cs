using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Logic.Models;
using MusicLibrary.PandaPlayer.Events;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.Events.SongEvents;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		private const int ExplorerSongListIndex = 0;
		private const int PlaylistSongListIndex = 1;

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

		public ISongPlaylistViewModel Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private int selectedSongListIndex;

		public int SelectedSongListIndex
		{
			get => selectedSongListIndex;
			set
			{
				Set(ref selectedSongListIndex, value);
				ActiveDisc = (SelectedSongListIndex == ExplorerSongListIndex) ? LibraryExplorerViewModel.SelectedDisc : PlaylistActiveDisc;
			}
		}

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

		private DiscModel PlaylistActiveDisc => Playlist.CurrentSong?.Disc ?? Playlist.PlayingDisc;

		public ICommand LoadCommand { get; }

		public ICommand ReversePlayingCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ApplicationViewModel(IApplicationViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
		{
			ViewModelHolder = viewModelHolder ?? throw new ArgumentNullException(nameof(viewModelHolder));
			MusicPlayerViewModel = musicPlayerViewModel ?? throw new ArgumentNullException(nameof(musicPlayerViewModel));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			LoadCommand = new RelayCommand(Load);
			ReversePlayingCommand = new RelayCommand(ReversePlaying);
			ShowLibraryStatisticsCommand = new AsyncRelayCommand(ShowLibraryStatistics);

			Messenger.Default.Register<PlaySongsListEventArgs>(this, OnPlaySongList);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, OnPlayPlaylistStartingFromSong);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => SwitchToExplorerSongList());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => NavigateLibraryExplorerToDisc(e.Disc));
		}

		public void Load()
		{
			Messenger.Default.Send(new ApplicationLoadedEventArgs());

			// Setting playlist active if some previous playlist was loaded.
			if (Playlist.Songs.Any())
			{
				SwitchToSongPlaylist();
			}
		}

		public async Task ShowLibraryStatistics()
		{
			await viewNavigator.ShowLibraryStatisticsView(CancellationToken.None);
		}

		private void OnPlaySongList(PlaySongsListEventArgs e)
		{
			Playlist.SetSongs(e.Songs);
			Playlist.SwitchToNextSong();
			ResetPlayer();
			SwitchToSongPlaylist();
		}

		private void OnPlayDiscFromSongLaunched(PlayDiscFromSongEventArgs message)
		{
			var disc = message.Song.Disc;

			Playlist.SetSongs(disc.Songs);
			Playlist.SwitchToSong(message.Song);
			ResetPlayer();
			SwitchToSongPlaylist();
		}

		private void OnPlayPlaylistStartingFromSong(PlayPlaylistStartingFromSongEventArgs message)
		{
			ResetPlayer();
			SwitchToSongPlaylist();
		}

		internal void ReversePlaying()
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

		private void ResetPlayer()
		{
			MusicPlayerViewModel.Stop();
			MusicPlayerViewModel.Play();
		}

		private void OnPlaylistSongChanged()
		{
			Title = Playlist.CurrentSong != null ? BuildCurrentTitle(Playlist.CurrentSong) : DefaultTitle;

			if (SelectedSongListIndex == PlaylistSongListIndex)
			{
				ActiveDisc = PlaylistActiveDisc;
			}
		}

		private string BuildCurrentTitle(SongModel song)
		{
			var songTitle = song.Artist != null ? Current($"{song.Artist.Name} - {song.Title}") : song.Title;
			return Current($"{Playlist.CurrentSongIndex + 1}/{Playlist.SongsNumber} - {songTitle}");
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

		private void SwitchToExplorerSongList()
		{
			SelectedSongListIndex = ExplorerSongListIndex;
		}

		private void SwitchToSongPlaylist()
		{
			SelectedSongListIndex = PlaylistSongListIndex;
		}

		private void NavigateLibraryExplorerToDisc(DiscModel disc)
		{
			LibraryExplorerViewModel.SwitchToDisc(disc);
			SwitchToExplorerSongList();
		}
	}
}
