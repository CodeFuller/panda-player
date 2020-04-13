using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.ViewModels.Player
{
	public interface ISongPlaybacksRegistrator
	{
		Task RegisterPlaybackStart(Song song);

		Task RegisterPlaybackFinish(Song song);
	}
}
