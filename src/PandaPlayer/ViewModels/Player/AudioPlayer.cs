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
		private enum PlayerState
		{
			Playing,
			Paused,
			Stopped,
		}

		private const double SongPositionUpdateIntervalInMilliseconds = 200;

		private readonly IMediaPlayerFacade mediaPlayer;

		private readonly ITimerFacade timer;

		public event PropertyChangedEventHandler PropertyChanged;

		private PlayerState state;

		private PlayerState State
		{
			get => state;
			set
			{
				state = value;

				// We raise PropertyChanged event for SongPosition only for Stopped state, because this is the only case when song position visibly changes.
				// When state is changed to Playing or Paused, song position remains the same.
				// It could seem that this condition could be removed, but it is not so.
				// When playing is resumed (switched from Paused to Playing state), mediaPlayer.Position will incorrectly return zero during short period of time.
				// This is not a problem if song position is updated by the timer (enough time passes for mediaPlayer.Position update),
				// but it becomes a problem if SongPosition is queried instantly due to PropertyChanged event.
				if (state == PlayerState.Stopped)
				{
					OnPropertyChanged(nameof(SongPosition));
				}
			}
		}

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
			get => (State == PlayerState.Playing || State == PlayerState.Paused) ? mediaPlayer.Position : TimeSpan.Zero;
			set
			{
				if (State == PlayerState.Playing || State == PlayerState.Paused)
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

			this.mediaPlayer.MediaOpened += MediaPlayerOnMediaOpened;
			this.mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;

			this.timer.Elapsed += Timer_Elapsed;
			this.timer.Interval = SongPositionUpdateIntervalInMilliseconds;
		}

		public void Open(Uri contentUri)
		{
			mediaPlayer.Open(contentUri);
		}

		public void Play()
		{
			mediaPlayer.Play();
			timer.Start();

			State = PlayerState.Playing;
		}

		public void Pause()
		{
			mediaPlayer.Pause();
			timer.Stop();

			State = PlayerState.Paused;
		}

		public void Stop()
		{
			mediaPlayer.Stop();
			timer.Stop();

			State = PlayerState.Stopped;
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
			mediaPlayer.Close();
			timer.Stop();

			State = PlayerState.Stopped;

			SongLength = TimeSpan.Zero;

			Messenger.Default.Send(new SongMediaFinishedEventArgs());
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
