using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using CF.MusicLibrary.AlbumPreprocessor.Bootstrap;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.AlbumPreprocessor.Views;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Object lifetime equals to the host process lifetime.")]
	public partial class App : Application
	{
		private bool initialized;

		/// <summary>
		/// Property Injection for IApplicationBootstrapper.
		/// </summary>
		internal IApplicationBootstrapper Bootstrapper { get; set; } = new ApplicationBootstrapper();

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			//	Catching all unhandled exceptions from the main UI thread.
			Application.Current.DispatcherUnhandledException += App_CatchedUnhandledUIException;

			AppDomain.CurrentDomain.UnhandledException += App_CatchedUnhandledAppException;

			Bootstrapper.Run();
			MainWindowModel rootViewModel = Bootstrapper.GetRootViewModel<MainWindowModel>(ConfigurationManager.AppSettings["AppDataPath"]);
			rootViewModel.LoadDefaultContent();
			MainWindow mainWindow = new MainWindow(rootViewModel);
			mainWindow.Show();
		}

		private void App_Activated(object sender, EventArgs e)
		{
			if (!initialized)
			{
				initialized = true;
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
