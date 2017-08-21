using System.Threading.Tasks;
using CF.MusicLibrary.PandaPlayer.Player;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IMusicPlayerViewModel
	{
		ISongPlaylist Playlist { get; }

		double Volume { get; set; }

		Task Play();

		void Pause();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "'Stop' is the best name in current semantics")]
		void Stop();

		void SetCurrentSongProgress(double progress);
	}
}
