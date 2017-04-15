using System;
using System.Windows;
using System.Windows.Threading;
using CF.Library.Wpf;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.AlbumPreprocessor.Views;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : WpfApplication<MainWindowModel>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object lifetime equals to the host process lifetime")]
		public App() : base(new Bootstrapper())
		{
		}

		protected override void Run(MainWindowModel rootViewModel)
		{
			if (rootViewModel == null)
			{
				throw new ArgumentNullException(nameof(rootViewModel));
			}

			//	Catching all unhandled exceptions from the main UI thread.
			Application.Current.DispatcherUnhandledException += App_CatchedUnhandledUIException;

			rootViewModel.LoadDefaultContent();
			MainWindow mainWindow = new MainWindow(rootViewModel);
			mainWindow.Show();
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
