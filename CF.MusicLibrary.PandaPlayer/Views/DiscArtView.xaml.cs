using System.Windows.Controls;
using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for DiscArtView.xaml
	/// </summary>
	public partial class DiscArtView : UserControl
	{
		private IDiscArtViewModel ViewModel => DataContext.GetViewModel<IDiscArtViewModel>();

		public DiscArtView()
		{
			InitializeComponent();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Method name follows convention for event handlers")]
		public async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				await ViewModel.EditDiscArt();
			}
		}
	}
}
