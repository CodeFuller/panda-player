using System;
using System.Windows;
using System.Windows.Media;

namespace PandaPlayer.ViewModels.Player
{
	public class MediaPlayerFacade : IMediaPlayerFacade
	{
		private readonly MediaPlayer mediaPlayer;

		public event EventHandler MediaOpened
		{
			add => mediaPlayer.MediaOpened += value;
			remove => mediaPlayer.MediaOpened -= value;
		}

		public event EventHandler MediaEnded
		{
			add => mediaPlayer.MediaEnded += value;
			remove => mediaPlayer.MediaEnded -= value;
		}

		public TimeSpan Position
		{
			get => mediaPlayer.Position;
			set => mediaPlayer.Position = value;
		}

		public Duration NaturalDuration => mediaPlayer.NaturalDuration;

		public double Volume
		{
			get => mediaPlayer.Volume;
			set => mediaPlayer.Volume = value;
		}

		public MediaPlayerFacade()
		{
			mediaPlayer = new MediaPlayer();
		}

		public void Open(Uri source)
		{
			mediaPlayer.Open(source);
		}

		public void Play()
		{
			mediaPlayer.Play();
		}

		public void Pause()
		{
			mediaPlayer.Pause();
		}

		public void Stop()
		{
			mediaPlayer.Stop();
		}

		public void Close()
		{
			mediaPlayer.Close();
		}
	}
}
