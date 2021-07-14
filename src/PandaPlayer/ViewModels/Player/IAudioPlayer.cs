using System;
using System.ComponentModel;

namespace PandaPlayer.ViewModels.Player
{
	public interface IAudioPlayer : INotifyPropertyChanged
	{
		event EventHandler<SongMediaFinishedEventArgs> SongMediaFinished;

		TimeSpan CurrSongLength { get; }

		TimeSpan CurrSongPosition { get; set; }

		double Volume { get; set; }

		void SetCurrentSongContentUri(Uri contentUri);

		void Play();

		void Pause();

#pragma warning disable CA1716 // Identifiers should not match keywords - 'Stop' is the best name in current semantics
		void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
	}
}
