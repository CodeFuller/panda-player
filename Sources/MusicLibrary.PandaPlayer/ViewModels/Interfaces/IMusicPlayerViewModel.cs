using System;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
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

#pragma warning disable CA1716 // Identifiers should not match keywords - 'Stop' is the best name in current semantics
		void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
	}
}
