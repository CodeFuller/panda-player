using System;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IMusicPlayerViewModel
	{
		bool IsPlaying { get; }

		TimeSpan CurrSongLength { get; }

		TimeSpan CurrSongElapsed { get; }

		double CurrSongProgress { get; set; }

		ISongPlaylistViewModel Playlist { get; }

		Song CurrentSong { get; }

		double Volume { get; set; }

		Task Play();

		void Pause();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is the best name in current semantics")]
		void Stop();
	}
}
