using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	internal interface IDiscAdviserViewModel
	{
		string CurrentAdviseAnnouncement { get; }

		ICommand PlayCurrentAdviseCommand { get; }

		ICommand SwitchToNextAdviseCommand { get; }
	}
}
