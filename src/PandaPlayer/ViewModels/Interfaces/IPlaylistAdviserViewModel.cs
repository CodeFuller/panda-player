using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	internal interface IPlaylistAdviserViewModel
	{
		string CurrentAdviseAnnouncement { get; }

		ICommand PlayCurrentAdviseCommand { get; }

		ICommand SwitchToNextAdviseCommand { get; }
	}
}
