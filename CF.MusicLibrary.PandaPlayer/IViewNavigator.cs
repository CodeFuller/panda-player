using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer
{
	public interface IViewNavigator
	{
		void BringApplicationToFront();

		void ShowRateDiscViewDialog(RateDiscViewModel viewModel);

		void ShowRateReminderViewDialog(RateDiscViewModel viewModel);

		void ShowDiscPropertiesView(Disc disc);

		void ShowSongPropertiesView(IEnumerable<Song> songs);
	}
}
