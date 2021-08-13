using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Core.Models;
using PandaPlayer.Events.SongEvents;

namespace PandaPlayer.ViewModels.Player
{
	public class SongPlayerViewModel : ViewModelBase, ISongPlayerViewModel
	{
		private readonly IAudioPlayer audioPlayer;

		private readonly ISongPlaybacksRegistrar playbacksRegistrar;

		private SongModel CurrentSong { get; set; }

		public TimeSpan SongLength => audioPlayer.SongLength;

		public TimeSpan SongPosition
		{
			get => audioPlayer.SongPosition;
			private set
			{
				audioPlayer.SongPosition = value;
				RaisePropertyChanged();
			}
		}

		public double SongProgress
		{
			get
			{
				var songElapsed = SongPosition;
				var songLength = SongLength;

				if (songLength == TimeSpan.Zero)
				{
					return 0;
				}

				return 100 * songElapsed.TotalMilliseconds / songLength.TotalMilliseconds;
			}

			set
			{
				SongPosition = TimeSpan.FromMilliseconds(SongLength.TotalMilliseconds * value);
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

		public string ReversePlayingKind => IsPlaying ? "Pause" : "Play";

		public SongPlayerViewModel(IAudioPlayer audioPlayer, ISongPlaybacksRegistrar playbacksRegistrar)
		{
			this.audioPlayer = audioPlayer ?? throw new ArgumentNullException(nameof(audioPlayer));
			this.playbacksRegistrar = playbacksRegistrar ?? throw new ArgumentNullException(nameof(playbacksRegistrar));

			Messenger.Default.Register<SongMediaFinishedEventArgs>(this, _ => OnSongFinished(CancellationToken.None));
			this.audioPlayer.PropertyChanged += AudioPlayer_PropertyChanged;
		}

		public async Task Play(SongModel song, CancellationToken cancellationToken)
		{
			audioPlayer.Open(song.ContentUri);
			audioPlayer.Play();

			IsPlaying = true;
			CurrentSong = song;

			await playbacksRegistrar.RegisterPlaybackStart(song, cancellationToken);
		}

		public void ReversePlaying()
		{
			if (IsPlaying)
			{
				audioPlayer.Pause();
				IsPlaying = false;
			}
			else
			{
				audioPlayer.Play();
				IsPlaying = true;
			}
		}

		public void StopPlaying()
		{
			audioPlayer.Stop();

			IsPlaying = false;
			CurrentSong = null;
		}

		private async void OnSongFinished(CancellationToken cancellationToken)
		{
			if (CurrentSong == null)
			{
				return;
			}

			await playbacksRegistrar.RegisterPlaybackFinish(CurrentSong, cancellationToken);

			IsPlaying = false;
			CurrentSong = null;

			// We cannot use the same event SongMediaFinishedEventArgs both for SongPlayerViewModel and PlaylistPlayerViewModel because order of handling matters.
			// SongPlayerViewModel should handle playback finish first, because PlaylistPlayerViewModel will change current song and register playback start.
			Messenger.Default.Send(new SongPlaybackFinishedEventArgs());
		}

		private void AudioPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IAudioPlayer.SongLength):
					RaisePropertyChanged(nameof(SongLength));
					RaisePropertyChanged(nameof(SongProgress));
					return;

				case nameof(IAudioPlayer.SongPosition):
					RaisePropertyChanged(nameof(SongPosition));
					RaisePropertyChanged(nameof(SongProgress));
					return;
			}
		}
	}
}
