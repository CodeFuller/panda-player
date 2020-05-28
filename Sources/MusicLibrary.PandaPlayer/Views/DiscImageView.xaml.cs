using System.Windows.Controls;
using System.Windows.Input;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for DiscImageView.xaml.
	/// </summary>
	public partial class DiscImageView : UserControl
	{
		private IDiscImageViewModel ViewModel => DataContext.GetViewModel<IDiscImageViewModel>();

		public DiscImageView()
		{
			InitializeComponent();
		}

#pragma warning disable CA1707 // Identifiers should not contain underscores - Method name follows convention for event handlers
		public void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#pragma warning restore CA1707 // Identifiers should not contain underscores
		{
			if (e.ClickCount == 2)
			{
				ViewModel.EditDiscImage();
			}
		}
	}
}
