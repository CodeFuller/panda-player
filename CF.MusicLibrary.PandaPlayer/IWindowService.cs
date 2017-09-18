using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer
{
	public interface IWindowService
	{
		void BringApplicationToFront();

		void ShowRateDiscViewDialog(RateDiscViewModel viewModel);

		void ShowRateReminderViewDialog(RateDiscViewModel viewModel);

		bool ShowSongPropertiesView(IEditSongPropertiesViewModel viewModel);
	}
}
