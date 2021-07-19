﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CodeFuller.Library.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongListEvents;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.ViewModels.Player
{
	public class MusicPlayerViewModel : ViewModelBase, IMusicPlayerViewModel
	{
		private readonly IPlaylistViewModel playlist;
		private readonly ISongPlaybacksRegistrator playbacksRegistrator;
		private readonly IAudioPlayer audioPlayer;

		private bool isPlaying;

		private bool IsPlaying
		{
			get => isPlaying;
			set
			{
				Set(ref isPlaying, value);
				RaisePropertyChanged(nameof(ReversePlayingKind));
			}
		}

		public TimeSpan CurrentSongLength => audioPlayer.CurrSongLength;

		public TimeSpan CurrentSongElapsed => audioPlayer.CurrSongPosition;

		public double CurrentSongProgress
		{
			get
			{
				var currentSongElapsed = CurrentSongElapsed;
				var currentSongLength = CurrentSongLength;

				if (currentSongLength == TimeSpan.Zero)
				{
					return 0;
				}

				return Math.Round(100 * currentSongElapsed.TotalMilliseconds / currentSongLength.TotalMilliseconds);
			}

			set
			{
				audioPlayer.CurrSongPosition = TimeSpan.FromMilliseconds(CurrentSongLength.TotalMilliseconds * value);
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

		/// <summary>
		/// Gets or sets the song, which is currently loaded into audio player.
		/// </summary>
		private SongModel CurrentSong { get; set; }

		public string ReversePlayingKind => IsPlaying ? "Pause" : "Play";

		public ICommand ReversePlayingCommand { get; }

		public MusicPlayerViewModel(IPlaylistViewModel playlist, IAudioPlayer audioPlayer, ISongPlaybacksRegistrator playbacksRegistrator)
		{
			this.playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
			this.audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
			this.playbacksRegistrator = playbacksRegistrator ?? throw new ArgumentNullException(nameof(playbacksRegistrator));

			this.audioPlayer.PropertyChanged += AudioPlayer_PropertyChanged;
			this.audioPlayer.SongMediaFinished += (s, e) => AudioPlayer_SongFinished(CancellationToken.None);

			ReversePlayingCommand = new AsyncRelayCommand(() => ReversePlaying(CancellationToken.None));
		}

		public async Task Play(CancellationToken cancellationToken)
		{
			if (CurrentSong == null)
			{
				var currentSong = playlist.CurrentSong;
				if (currentSong == null)
				{
					return;
				}

				await SwitchToNewSong(currentSong, cancellationToken);
			}

			IsPlaying = true;
			audioPlayer.Play();
		}

		private Task Pause()
		{
			IsPlaying = false;
			audioPlayer.Pause();

			return Task.CompletedTask;
		}

		public Task ReversePlaying(CancellationToken cancellationToken)
		{
			return IsPlaying ? Pause() : Play(cancellationToken);
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

		private async void AudioPlayer_SongFinished(CancellationToken cancellationToken)
		{
			var currentSong = playlist.CurrentSong;
			if (currentSong != null)
			{
				await playbacksRegistrator.RegisterPlaybackFinish(currentSong, cancellationToken);
			}

			CurrentSong = null;

			await playlist.SwitchToNextSong(cancellationToken);
			if (playlist.CurrentSong == null)
			{
				// We have reached the end of playlist.
				IsPlaying = false;
				Messenger.Default.Send(new PlaylistFinishedEventArgs(playlist.Songs));
				return;
			}

			// Play next song from the playlist.
			await Play(cancellationToken);
		}

		private async Task SwitchToNewSong(SongModel newSong, CancellationToken cancellationToken)
		{
			CurrentSong = newSong;

			audioPlayer.SetCurrentSongContentUri(newSong.ContentUri);
			await playbacksRegistrator.RegisterPlaybackStart(newSong, cancellationToken);
		}

		private void AudioPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IAudioPlayer.CurrSongLength):
					RaisePropertyChanged(nameof(CurrentSongLength));
					RaisePropertyChanged(nameof(CurrentSongProgress));
					return;

				case nameof(IAudioPlayer.CurrSongPosition):
					RaisePropertyChanged(nameof(CurrentSongElapsed));
					RaisePropertyChanged(nameof(CurrentSongProgress));
					return;
			}
		}
	}
}
