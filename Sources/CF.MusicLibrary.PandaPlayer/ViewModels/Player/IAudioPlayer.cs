using System;
using System.ComponentModel;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	public interface IAudioPlayer : INotifyPropertyChanged
	{
		event EventHandler<SongMediaFinishedEventArgs> SongMediaFinished;

		bool IsPlaying { get; }

		TimeSpan CurrSongLength { get; }

		TimeSpan CurrSongPosition { get; set; }

		double Volume { get; set; }

		void SetCurrentSongFile(string fileName);

		void Play();

		void Pause();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "'Stop' is the best name in current semantics")]
		void Stop();
	}
}
