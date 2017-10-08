using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.Adviser;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	public interface IDiscAdviserViewModel
	{
		AdvisedPlaylist CurrentAdvise { get; }

		string CurrentAdviseAnnouncement { get; }

		ICommand PlayCurrentAdviseCommand { get; }

		ICommand SwitchToNextAdviseCommand { get; }
	}
}
