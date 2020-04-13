using System;
using System.Windows;

namespace MusicLibrary.PandaPlayer.ViewModels.Player
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

#pragma warning disable CA1716 // Identifiers should not match keywords - 'Stop' is the best name in current semantics
		void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords

		void Close();
	}
}
