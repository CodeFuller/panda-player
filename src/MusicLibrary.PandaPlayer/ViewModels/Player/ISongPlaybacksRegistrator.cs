using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.ViewModels.Player
{
	public interface ISongPlaybacksRegistrator
	{
		Task RegisterPlaybackStart(SongModel song, CancellationToken cancellationToken);

		Task RegisterPlaybackFinish(SongModel song, CancellationToken cancellationToken);
	}
}
