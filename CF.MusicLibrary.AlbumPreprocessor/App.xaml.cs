using System;
using System.Windows;
using System.Windows.Threading;
using CF.MusicLibrary.AlbumPreprocessor.Views;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private bool initialized;

		private MainWindow GetMainWindow()
		{
			return (MainWindow)MainWindow;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			//	Catching all unhandled exceptions from the main UI thread.
			Application.Current.DispatcherUnhandledException += App_CatchedUnhandledUIException;

			AppDomain.CurrentDomain.UnhandledException += App_CatchedUnhandledAppException;
		}

		private void App_Activated(object sender, EventArgs e)
		{
			if (!initialized)
			{
				initialized = true;

				GetMainWindow().Model.LoadCurrentAlbums();
			}
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

		private void App_CatchedUnhandledAppException(object sender, UnhandledExceptionEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				MessageBox.Show("Some error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			});
		}
	}
}
