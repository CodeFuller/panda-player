using System;
using System.Windows;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for RateDiscView.xaml
	/// </summary>
	public partial class RateDiscView : Window
	{
		public RateDiscView()
		{
			InitializeComponent();
		}

		private async void RateButton_Click(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as RateDiscViewModel;
			if (viewModel == null)
			{
				throw new InvalidOperationException("ViewModel is not set");
			}

			DialogResult = true;
			await viewModel.Save();
		}
	}
}
