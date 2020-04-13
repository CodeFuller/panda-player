using System.Windows;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Views
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
