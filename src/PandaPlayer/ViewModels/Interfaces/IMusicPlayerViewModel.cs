using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IMusicPlayerViewModel
	{
		TimeSpan CurrentSongLength { get; }

		TimeSpan CurrentSongElapsed { get; }

		double CurrentSongProgress { get; set; }

		IPlaylistViewModel Playlist { get; }

		double Volume { get; set; }

		Task Play(CancellationToken cancellationToken);

		string ReversePlayingKind { get; }

		ICommand ReversePlayingCommand { get; }

		Task ReversePlaying(CancellationToken cancellationToken);

#pragma warning disable CA1716 // Identifiers should not match keywords - 'Stop' is the best name in current semantics
		void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
	}
}
