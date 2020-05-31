using System.Windows.Controls;
using System.Windows.Input;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Views
{
	public partial class MusicPlayerView : UserControl
	{
		private IMusicPlayerViewModel ViewModel => DataContext.GetViewModel<IMusicPlayerViewModel>();

		public MusicPlayerView()
		{
			InitializeComponent();
		}

		private void CurrSongProgressBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ProgressBar progressBar = sender as ProgressBar;
			if (progressBar == null)
			{
				return;
			}

			ViewModel.CurrentSongProgress = GetProgressBarClickValue(progressBar, e);
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
