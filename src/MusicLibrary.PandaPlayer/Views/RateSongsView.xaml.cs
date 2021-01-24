using System.Threading;
using System.Windows;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Views
{
	public partial class RateSongsView : Window
	{
		private IRateSongsViewModel ViewModel => DataContext.GetViewModel<IRateSongsViewModel>();

		public RateSongsView()
		{
			InitializeComponent();
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			await ViewModel.Save(CancellationToken.None);
		}
	}
}
