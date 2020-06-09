using System;
using System.Windows;
using System.Windows.Threading;
using CF.Library.Wpf;
using MusicLibrary.DiscAdder.ViewModels;
using MusicLibrary.DiscAdder.Views;

namespace MusicLibrary.DiscAdder
{
	internal partial class App : WpfApplication<ApplicationViewModel>
	{
		public App()
			: base(new ApplicationBootstrapper())
		{
		}

		protected override void Run(ApplicationViewModel rootViewModel)
		{
			if (rootViewModel == null)
			{
				throw new ArgumentNullException(nameof(rootViewModel));
			}

			// Catching all unhandled exceptions from the main UI thread.
			Application.Current.DispatcherUnhandledException += App_CatchedUnhandledUIException;

			rootViewModel.Load();
			ApplicationView appView = new ApplicationView(rootViewModel);
			appView.Show();
		}

		private void App_CatchedUnhandledUIException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			string errorMessage = e.Exception.ToString();

			Application.Current.Dispatcher.Invoke(() =>
			{
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			});

			e.Handled = true;
		}
	}
}
