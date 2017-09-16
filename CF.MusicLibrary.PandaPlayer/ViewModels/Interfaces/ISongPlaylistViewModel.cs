using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface ISongPlaylistViewModel : ISongListViewModel
	{
		Song CurrentSong { get; }

		void SwitchToNextSong();

		void SwitchToSong(Song song);
	}
}
