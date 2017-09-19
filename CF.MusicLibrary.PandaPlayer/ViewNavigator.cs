using System;
using System.Collections.Generic;
using System.Windows;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.Views;

namespace CF.MusicLibrary.PandaPlayer
{
	public class ViewNavigator : IViewNavigator
	{
		private static Window ApplicationWindow => Application.Current.MainWindow;

		private readonly IEditDiscPropertiesViewModel editDiscPropertiesViewModel;
		private readonly IEditSongPropertiesViewModel editSongPropertiesViewModel;

		public ViewNavigator(IEditDiscPropertiesViewModel editDiscPropertiesViewModel, IEditSongPropertiesViewModel editSongPropertiesViewModel)
		{
			if (editDiscPropertiesViewModel == null)
			{
				throw new ArgumentNullException(nameof(editDiscPropertiesViewModel));
			}
			if (editSongPropertiesViewModel == null)
			{
				throw new ArgumentNullException(nameof(editSongPropertiesViewModel));
			}

			this.editDiscPropertiesViewModel = editDiscPropertiesViewModel;
			this.editSongPropertiesViewModel = editSongPropertiesViewModel;
		}

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

		public void ShowDiscPropertiesView(Disc disc)
		{
			editDiscPropertiesViewModel.Load(disc);
			ShowDialog<EditDiscPropertiesView>(editDiscPropertiesViewModel);
		}

		public void ShowSongPropertiesView(IEnumerable<Song> songs)
		{
			editSongPropertiesViewModel.Load(songs);
			ShowDialog<EditSongPropertiesView>(editSongPropertiesViewModel);
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
