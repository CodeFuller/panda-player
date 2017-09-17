using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel : ISongListViewModel
	{
		Song CurrentSong { get; }

		/// <summary>
		/// If all songs in playlist belong to one Disc, returns this Disc.
		/// Returns null otherwise.
		/// </summary>
		Disc PlayedDisc { get; }

		void SwitchToNextSong();

		void SwitchToSong(Song song);
	}
}
