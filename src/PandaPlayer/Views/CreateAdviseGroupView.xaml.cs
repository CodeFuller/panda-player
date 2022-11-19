using System.Windows;
using System.Windows.Input;
using PandaPlayer.Views.Extensions;

namespace PandaPlayer.Views
{
	public partial class CreateAdviseGroupView : Window
	{
		public CreateAdviseGroupView()
		{
			InitializeComponent();
		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.IsValid();
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DialogResult = true;
			e.Handled = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
