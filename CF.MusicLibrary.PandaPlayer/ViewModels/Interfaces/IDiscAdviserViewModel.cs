using System.Windows.Input;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscAdviserViewModel
	{
		Disc CurrentDisc { get; }

		string CurrentDiscAnnouncement { get; }

		ICommand PlayCurrentDiscCommand { get; }

		ICommand SwitchToNextDiscCommand { get; }
	}
}
