using System.Windows.Input;
using MusicLibrary.PandaPlayer.Adviser;

namespace MusicLibrary.PandaPlayer.ViewModels.Interfaces
{
	internal interface IDiscAdviserViewModel
	{
		AdvisedPlaylist CurrentAdvise { get; }

		string CurrentAdviseAnnouncement { get; }

		ICommand PlayCurrentAdviseCommand { get; }

		ICommand SwitchToNextAdviseCommand { get; }
	}
}
