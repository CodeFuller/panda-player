using System.Windows;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.Views;

namespace CF.MusicLibrary.PandaPlayer
{
	public class WindowService : IWindowService
	{
		private static Window ApplicationWindow => Application.Current.MainWindow;

		public void ShowRateDiscViewDialog(RateDiscViewModel viewModel)
		{
			if (!ShowDialog<RateDiscView>(viewModel))
			{
				viewModel.SelectedRating = null;
			}
		}

		public void BringApplicationToFront()
		{
			ApplicationWindow.Show();
			ApplicationWindow.Activate();
		}

		public void ShowRateReminderViewDialog(RateDiscViewModel viewModel)
		{
			ShowDialog<RateReminderView>(viewModel);
		}

		public bool ShowSongPropertiesView(IEditSongPropertiesViewModel viewModel)
		{
			return ShowDialog<EditSongPropertiesView>(viewModel);
		}

		private static bool ShowDialog<TDialogView>(object dataContext) where TDialogView : Window, new()
		{
			TDialogView dialogView = new TDialogView
			{
				DataContext = dataContext,
				Owner = ApplicationWindow,
			};

			return dialogView.ShowDialog() ?? false;
		}
	}
}
