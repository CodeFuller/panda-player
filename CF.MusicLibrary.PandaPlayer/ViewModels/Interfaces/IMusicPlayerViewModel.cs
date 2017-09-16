using System.Threading.Tasks;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IMusicPlayerViewModel
	{
		ISongPlaylistViewModel Playlist { get; }

		double Volume { get; set; }

		bool IsPlaying { get; }

		Task Play();

		void Pause();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "'Resume' is the best name in current semantics")]
		void Resume();

		void SetCurrentSongProgress(double progress);
	}
}
