using System.Threading;
using System.Windows;
using PandaPlayer.ViewModels.Interfaces;

namespace PandaPlayer.Views
{
	public partial class DeleteContentView : Window
	{
		private IDeleteContentViewModel ViewModel => DataContext.GetViewModel<IDeleteContentViewModel>();

		public DeleteContentView()
		{
			InitializeComponent();
		}

		private async void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			await ViewModel.Delete(CancellationToken.None);
		}
	}
}
