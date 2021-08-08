using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using PandaPlayer.Facades;

namespace PandaPlayer.ViewModels.Player
{
	internal class AudioPlayer : IAudioPlayer
	{
		private const double SongPositionUpdateInterval = 200;

		private readonly IMediaPlayerFacade mediaPlayer;

		private readonly ITimerFacade timer;

		public event PropertyChangedEventHandler PropertyChanged;

		private bool IsPlaying { get; set; }

		private TimeSpan currentSongLength;

		public TimeSpan SongLength
		{
			get => currentSongLength;
			private set
			{
				currentSongLength = value;
				OnPropertyChanged();
			}
		}

		public TimeSpan SongPosition
		{
			get => IsPlaying ? mediaPlayer.Position : TimeSpan.Zero;
			set
			{
				if (IsPlaying)
				{
					mediaPlayer.Position = value;
				}

				OnPropertyChanged();
			}
		}

		public double Volume
		{
			get => mediaPlayer.Volume;
			set => mediaPlayer.Volume = value;
		}

		public AudioPlayer(IMediaPlayerFacade mediaPlayer, ITimerFacade timer)
		{
			this.mediaPlayer = mediaPlayer ?? throw new ArgumentNullException(nameof(mediaPlayer));
			this.timer = timer ?? throw new ArgumentNullException(nameof(timer));

			this.timer = timer;
			this.timer.Elapsed += Timer_Elapsed;
			this.timer.Interval = SongPositionUpdateInterval;

			this.mediaPlayer.MediaOpened += MediaPlayerOnMediaOpened;
			this.mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;
		}

		public void Open(Uri contentUri)
		{
			mediaPlayer.Open(contentUri);
		}

		public void Play()
		{
			mediaPlayer.Play();
			timer.Start();

			IsPlaying = true;
		}

		public void Pause()
		{
			mediaPlayer.Pause();
			OnPlaybackInterrupted();
		}

		public void Stop()
		{
			mediaPlayer.Stop();
			OnPlaybackInterrupted();
		}

		private void OnPlaybackInterrupted()
		{
			timer.Stop();
			IsPlaying = false;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			OnPropertyChanged(nameof(SongPosition));
		}

		private void MediaPlayerOnMediaOpened(object sender, EventArgs eventArgs)
		{
			SongLength = mediaPlayer.NaturalDuration.TimeSpan;
		}

		private void MediaPlayerOnMediaEnded(object sender, EventArgs eventArgs)
		{
			OnPlaybackInterrupted();

			mediaPlayer.Close();

			SongPosition = TimeSpan.Zero;
			SongLength = TimeSpan.Zero;

			Messenger.Default.Send(new SongMediaFinishedEventArgs());
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
