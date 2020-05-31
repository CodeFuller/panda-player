using System;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IMusicPlayerViewModel
	{
		bool IsPlaying { get; }

		TimeSpan CurrentSongLength { get; }

		TimeSpan CurrentSongElapsed { get; }

		double CurrentSongProgress { get; set; }

		ISongPlaylistViewModel Playlist { get; }

		SongModel CurrentSong { get; }

		double Volume { get; set; }

		Task Play();

		Task Pause();

		Task ReversePlaying();

#pragma warning disable CA1716 // Identifiers should not match keywords - 'Stop' is the best name in current semantics
		void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
	}
}
