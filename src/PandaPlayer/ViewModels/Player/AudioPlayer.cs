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
		private const double SongPositionUpdateIntervalInMilliseconds = 200;

		private readonly IMediaPlayerFacade mediaPlayer;

		private readonly ITimerFacade timer;

		public event PropertyChangedEventHandler PropertyChanged;

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
			get => mediaPlayer.Position;
			set
			{
				mediaPlayer.Position = value;
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
		}

		public void Pause()
		{
			mediaPlayer.Pause();
			timer.Stop();
		}

		public void Stop()
		{
			mediaPlayer.Stop();
			OnPropertyChanged(nameof(SongPosition));

			timer.Stop();
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

			SongLength = TimeSpan.Zero;

			// SongPosition value was affected by Close() method on mediaPlayer.
			OnPropertyChanged(nameof(SongPosition));

			Messenger.Default.Send(new SongMediaFinishedEventArgs());
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
