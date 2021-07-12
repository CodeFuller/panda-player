using System.Windows.Input;
using PandaPlayer.Adviser;

namespace PandaPlayer.ViewModels.Interfaces
{
	internal interface IDiscAdviserViewModel
	{
		AdvisedPlaylist CurrentAdvise { get; }

		string CurrentAdviseAnnouncement { get; }

		ICommand PlayCurrentAdviseCommand { get; }

		ICommand SwitchToNextAdviseCommand { get; }
	}
}
