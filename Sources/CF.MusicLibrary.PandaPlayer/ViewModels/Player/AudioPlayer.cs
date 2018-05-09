using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using CF.Library.Core.Facades;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	class AudioPlayer : IAudioPlayer
	{
		private const double SongPostionUpdateInterval = 200;

		private readonly IMediaPlayerFacade mediaPlayer;

		private readonly ITimerFacade timer;

		private string currentSongFile;

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<SongMediaFinishedEventArgs> SongMediaFinished;

		public bool IsPlaying { get; private set; }

		private TimeSpan currSongLength;
		public TimeSpan CurrSongLength
		{
			get => currSongLength;
			private set
			{
				currSongLength = value;
				OnPropertyChanged();
			}
		}

		public TimeSpan CurrSongPosition
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
			this.timer.Interval = SongPostionUpdateInterval;

			this.mediaPlayer.MediaOpened += MediaPlayerOnMediaOpened;
			this.mediaPlayer.MediaEnded += MediaPlayerOnMediaEnded;
		}

		public void SetCurrentSongFile(string songFileName)
		{
			mediaPlayer.Open(new Uri(songFileName));
			currentSongFile = songFileName;
		}

		public void Play()
		{
			if (currentSongFile == null)
			{
				throw new InvalidOperationException("Current song is not set");
			}

			IsPlaying = true;
			mediaPlayer.Play();
			timer.Start();
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

		protected void OnPlaybackInterrupted()
		{
			IsPlaying = false;
			timer.Stop();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			OnPropertyChanged(nameof(CurrSongPosition));
		}

		private void MediaPlayerOnMediaOpened(object sender, EventArgs eventArgs)
		{
			CurrSongLength = mediaPlayer.NaturalDuration.TimeSpan;
		}

		private void MediaPlayerOnMediaEnded(object sender, EventArgs eventArgs)
		{
			currentSongFile = null;
			OnPlaybackInterrupted();
			mediaPlayer.Close();
			CurrSongPosition = TimeSpan.Zero;
			CurrSongLength = TimeSpan.Zero;
			SongMediaFinished?.Invoke(this, new SongMediaFinishedEventArgs());
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
