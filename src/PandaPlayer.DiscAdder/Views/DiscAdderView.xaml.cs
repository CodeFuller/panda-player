using System;
using System.Windows;
using CodeFuller.Library.Wpf.Extensions;
using PandaPlayer.DiscAdder.ViewModels.Interfaces;

namespace PandaPlayer.DiscAdder.Views
{
	public partial class DiscAdderView : Window
	{
		private IDiscAdderViewModel ViewModel => DataContext.GetViewModel<IDiscAdderViewModel>();

		public DiscAdderView()
		{
			InitializeComponent();

			Loaded += View_Loaded;
		}

		private void View_Loaded(object sender, RoutedEventArgs e)
		{
			ViewModel.OnRequestClose += ViewModel_OnRequestClose;

			Loaded -= View_Loaded;
		}

		private void ViewModel_OnRequestClose(object sender, EventArgs e)
		{
			ViewModel.OnRequestClose -= ViewModel_OnRequestClose;

			Close();
		}
	}
}
