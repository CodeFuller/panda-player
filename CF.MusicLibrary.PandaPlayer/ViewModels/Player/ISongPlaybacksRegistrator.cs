using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Player
{
	public interface ISongPlaybacksRegistrator
	{
		Task RegisterPlaybackStart(Song song);

		Task RegisterPlaybackFinish(Song song);
	}
}
