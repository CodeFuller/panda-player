using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer
{
	public interface IWindowService
	{
		void BringApplicationToFront();

		void ShowRateDiscViewDialog(RateDiscViewModel viewModel);

		void ShowRateReminderViewDialog(RateDiscViewModel viewModel);
	}
}
