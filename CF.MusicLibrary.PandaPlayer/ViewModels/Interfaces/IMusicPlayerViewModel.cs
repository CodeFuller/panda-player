using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

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

		/// <summary>
		/// Starts or resumes song playback.
		/// </summary>
		/// <returns></returns>
		Task Play();

		void Pause();
	}
}
