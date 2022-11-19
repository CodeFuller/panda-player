using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Player
{
	public interface ISongPlayerViewModel
	{
		TimeSpan SongLength { get; }

		TimeSpan SongPosition { get; }

		double SongProgress { get; set; }

		double Volume { get; set; }

		string ReversePlayingKind { get; }

		Task Play(SongModel song, CancellationToken cancellationToken);

		void ReversePlaying();

		void StopPlaying();
	}
}
