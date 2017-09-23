using System;
using System.Windows.Controls;
using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for MusicPlayerView.xaml
	/// </summary>
	public partial class MusicPlayerView : UserControl
	{
		public MusicPlayerView()
		{
			InitializeComponent();
		}

		public IMusicPlayerViewModel ViewModel
		{
			get
			{
				var viewModel = DataContext as IMusicPlayerViewModel;
				if (viewModel == null)
				{
					throw new InvalidOperationException("MusicPlayerViewModel is not set");
				}

				return viewModel;
			}
		}
		

		private void CurrSongProgressBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ProgressBar progressBar = sender as ProgressBar;
			if (progressBar == null)
			{
				return;
			}

			ViewModel.CurrSongProgress = GetProgressBarClickValue(progressBar, e);
		}

		private void VolumeBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ProgressBar volumeBar = sender as ProgressBar;
			if (volumeBar == null)
			{
				return;
			}

			ViewModel.Volume = GetProgressBarClickValue(volumeBar, e);
		}

		private static double GetProgressBarClickValue(ProgressBar progressBar, MouseButtonEventArgs e)
		{
			double mousePos = e.GetPosition(progressBar).X;
			return mousePos / progressBar.ActualWidth;
		}
	}
}
