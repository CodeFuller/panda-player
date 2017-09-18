using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscAdviserViewModel
	{
		Disc CurrentDisc { get; }

		void Load();
	}
}
