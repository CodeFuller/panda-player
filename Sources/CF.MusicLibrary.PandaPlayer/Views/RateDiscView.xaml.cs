using System.Windows;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for RateDiscView.xaml
	/// </summary>
	public partial class RateDiscView : Window
	{
		private IRateDiscViewModel ViewModel => DataContext.GetViewModel<IRateDiscViewModel>();

		public RateDiscView()
		{
			InitializeComponent();
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			await ViewModel.Save();
		}
	}
}
