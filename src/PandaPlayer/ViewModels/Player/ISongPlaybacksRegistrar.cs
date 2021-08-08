using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Player
{
	public interface ISongPlaybacksRegistrar
	{
		Task RegisterPlaybackStart(SongModel song, CancellationToken cancellationToken);

		Task RegisterPlaybackFinish(SongModel song, CancellationToken cancellationToken);
	}
}
