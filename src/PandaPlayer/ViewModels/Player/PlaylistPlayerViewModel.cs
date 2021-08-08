using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.Player
{
	public class PlaylistPlayerViewModel : ViewModelBase, IPlaylistPlayerViewModel
	{
		private readonly IPlaylistViewModel playlist;

		private SongModel CurrentSong { get; set; }

		public ISongPlayerViewModel SongPlayerViewModel { get; }

		public ICommand ReversePlayingCommand { get; }

		public PlaylistPlayerViewModel(IPlaylistViewModel playlist, ISongPlayerViewModel songPlayerViewModel)
		{
			this.playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
			SongPlayerViewModel = songPlayerViewModel ?? throw new ArgumentNullException(nameof(songPlayerViewModel));

			ReversePlayingCommand = new AsyncRelayCommand(() => ReversePlaying(CancellationToken.None));

			Messenger.Default.Register<PlaySongsListEventArgs>(this, e => OnPlaySongList(e, CancellationToken.None));
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, _ => OnPlayPlaylistStartingFromSong(CancellationToken.None));
			Messenger.Default.Register<SongPlaybackFinishedEventArgs>(this, _ => OnSongPlaybackFinished(CancellationToken.None));
		}

		public async Task ReversePlaying(CancellationToken cancellationToken)
		{
			if (CurrentSong == null)
			{
				await StartPlaying(cancellationToken);
			}
			else
			{
				SongPlayerViewModel.ReversePlaying();
			}
		}

		private async Task StartPlaying(CancellationToken cancellationToken)
		{
			CurrentSong = playlist.CurrentSong;
			if (CurrentSong == null)
			{
				// No songs to play.
				return;
			}

			await SongPlayerViewModel.Play(CurrentSong, cancellationToken);
		}

		private void StopPlaying()
		{
			if (CurrentSong == null)
			{
				return;
			}

			SongPlayerViewModel.StopPlaying();
			CurrentSong = null;
		}

		private async void OnPlaySongList(PlaySongsListEventArgs e, CancellationToken cancellationToken)
		{
			StopPlaying();

			await playlist.SetPlaylistSongs(e.Songs, cancellationToken);

			await StartPlaying(cancellationToken);
		}

		private async void OnPlayPlaylistStartingFromSong(CancellationToken cancellationToken)
		{
			StopPlaying();

			await StartPlaying(cancellationToken);
		}

		private async void OnSongPlaybackFinished(CancellationToken cancellationToken)
		{
			CurrentSong = null;

			await playlist.SwitchToNextSong(cancellationToken);
			if (playlist.CurrentSong == null)
			{
				// We have reached the end of playlist.
				Messenger.Default.Send(new PlaylistFinishedEventArgs(playlist.Songs));
			}
			else
			{
				// Playing next song from the playlist.
				await StartPlaying(cancellationToken);
			}
		}
	}
}
