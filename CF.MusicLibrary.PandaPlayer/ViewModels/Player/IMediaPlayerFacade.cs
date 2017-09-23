using System;
using System.Windows;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	public interface IMediaPlayerFacade
	{
		event EventHandler MediaOpened;

		event EventHandler MediaEnded;

		TimeSpan Position { get; set; }

		Duration NaturalDuration { get; }

		double Volume { get; set; }

		void Open(Uri source);

		void Play();

		void Pause();
	}
}
