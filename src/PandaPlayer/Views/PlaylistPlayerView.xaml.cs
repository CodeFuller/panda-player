using System.Windows.Controls;
using System.Windows.Input;
using CodeFuller.Library.Wpf.Extensions;
using PandaPlayer.ViewModels.Interfaces;
using PandaPlayer.ViewModels.Player;

namespace PandaPlayer.Views
{
	public partial class PlaylistPlayerView : UserControl
	{
		private IPlaylistPlayerViewModel ViewModel => DataContext.GetViewModel<IPlaylistPlayerViewModel>();

		private ISongPlayerViewModel SongPlayerViewModel => ViewModel.SongPlayerViewModel;

		public PlaylistPlayerView()
		{
			InitializeComponent();
		}

		private void CurrentSongProgressBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not ProgressBar progressBar)
			{
				return;
			}

			SongPlayerViewModel.SongProgress = GetProgressBarClickValue(progressBar, e);
		}

		private void VolumeBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not ProgressBar volumeBar)
			{
				return;
			}

			SongPlayerViewModel.Volume = GetProgressBarClickValue(volumeBar, e);
		}

		private static double GetProgressBarClickValue(ProgressBar progressBar, MouseButtonEventArgs e)
		{
			var mousePos = e.GetPosition(progressBar).X;
			return mousePos / progressBar.ActualWidth;
		}
	}
}
