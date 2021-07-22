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

		double Volume { get; set; }

		Task Play(CancellationToken cancellationToken);

		string ReversePlayingKind { get; }

		ICommand ReversePlayingCommand { get; }

		Task ReversePlaying(CancellationToken cancellationToken);
	}
}
