using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Interfaces.Services;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Events.SongListEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.Player
{
	public class MusicPlayerViewModel : ViewModelBase, IMusicPlayerViewModel
	{
		private readonly ISongsService songsService;
		private readonly ISongPlaybacksRegistrator playbacksRegistrator;
		private readonly IAudioPlayer audioPlayer;

		private bool isPlaying;

		public bool IsPlaying
		{
			get => isPlaying;
			set => Set(ref isPlaying, value);
		}

		public TimeSpan CurrSongLength => audioPlayer.CurrSongLength;

		public TimeSpan CurrSongElapsed => audioPlayer.CurrSongPosition;

		public double CurrSongProgress
		{
			get
			{
				TimeSpan currSongElapsed = CurrSongElapsed;
				TimeSpan currSongLength = CurrSongLength;

				if (currSongLength == TimeSpan.Zero)
				{
					return 0;
				}

				return Math.Round(100 * currSongElapsed.TotalMilliseconds / currSongLength.TotalMilliseconds);
			}

			set
			{
				audioPlayer.CurrSongPosition = TimeSpan.FromMilliseconds(CurrSongLength.TotalMilliseconds * value);
				RaisePropertyChanged();
			}
		}

		public double Volume
		{
			get => audioPlayer.Volume;
			set
			{
				audioPlayer.Volume = value;
				RaisePropertyChanged();
			}
		}

		public ISongPlaylistViewModel Playlist { get; }

		/// <summary>
		/// Gets the song, which is currently loaded into audio player.
		/// </summary>
		public SongModel CurrentSong { get; private set; }

		public ICommand PlayCommand { get; }

		public ICommand PauseCommand { get; }

		public MusicPlayerViewModel(ISongsService songsService, ISongPlaylistViewModel playlist, IAudioPlayer audioPlayer, ISongPlaybacksRegistrator playbacksRegistrator)
		{
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.Playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
			this.audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
			this.playbacksRegistrator = playbacksRegistrator ?? throw new ArgumentNullException(nameof(playbacksRegistrator));

			this.audioPlayer.PropertyChanged += AudioPlayer_PropertyChanged;
			this.audioPlayer.SongMediaFinished += AudioPlayer_SongFinished;

			PlayCommand = new AsyncRelayCommand(Play);
			PauseCommand = new RelayCommand(Pause);
		}

		public async Task Play()
		{
			if (CurrentSong == null)
			{
				var currentSong = Playlist.CurrentSong;
				if (currentSong == null)
				{
					return;
				}

				await SwitchToNewSong(currentSong);
			}

			IsPlaying = true;
			audioPlayer.Play();
		}

		public void Pause()
		{
			IsPlaying = false;
			audioPlayer.Pause();
		}

		public void Stop()
		{
			if (IsPlaying)
			{
				IsPlaying = false;
				audioPlayer.Stop();
				CurrentSong = null;
			}
		}

		private async void AudioPlayer_SongFinished(object sender, SongMediaFinishedEventArgs eventArgs)
		{
			var currSong = Playlist.CurrentSong;
			if (currSong != null)
			{
				await playbacksRegistrator.RegisterPlaybackFinish(currSong, CancellationToken.None);
			}

			CurrentSong = null;

			Playlist.SwitchToNextSong();
			if (Playlist.CurrentSong == null)
			{
				// We have reached the end of playlist.
				IsPlaying = false;
				Messenger.Default.Send(new PlaylistFinishedEventArgs(Playlist));
				return;
			}

			// Play next song from the playlist.
			await Play();
		}

		private async Task SwitchToNewSong(SongModel newSong)
		{
			CurrentSong = newSong;

			audioPlayer.SetCurrentSongContentUri(newSong.ContentUri);
			await playbacksRegistrator.RegisterPlaybackStart(newSong, CancellationToken.None);
		}

		private void AudioPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IAudioPlayer.CurrSongLength):
					RaisePropertyChanged(nameof(CurrSongLength));
					RaisePropertyChanged(nameof(CurrSongProgress));
					return;

				case nameof(IAudioPlayer.CurrSongPosition):
					RaisePropertyChanged(nameof(CurrSongElapsed));
					RaisePropertyChanged(nameof(CurrSongProgress));
					return;
			}
		}
	}
}
