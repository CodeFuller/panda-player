using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CF.MusicLibrary.PandaPlayer.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for ApplicationView.xaml
	/// </summary>
	public partial class ApplicationView : Window
	{
		private readonly TaskbarIcon taskBarIcon;

		private ApplicationViewModel ViewModel => DataContext.GetViewModel<ApplicationViewModel>();

		public ApplicationView(ApplicationViewModel model)
		{
			InitializeComponent();
			DataContext = model;

			//  Initializing NotifyIcon
			taskBarIcon = (TaskbarIcon)FindResource("MainWindowNotifyIcon");

			//  When minimized button is pressed we don't want window to be minimized because
			//  it will cause all children windows (questions) also to be minimized.
			//  Instead we're keeping window in normal state and hiding it to system tray
			//  This code and HandleMessages() method were taken from http://stackoverflow.com/questions/13022921/how-to-cancel-the-wpf-form-minimize-event
			SourceInitialized += (sender, e) =>
			{
				var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
				source?.AddHook(WndProc);
			};

			var iconUri = new Uri("pack://application:,,,/Views/Icons/PandaPlayer.ico", UriKind.RelativeOrAbsolute);
			var icon = BitmapFrame.Create(iconUri);
			Icon = icon;
			taskBarIcon.IconSource = icon;

			model.PropertyChanged += ViewModel_PropertyChanged;
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ViewModel.Title))
			{
				taskBarIcon.ToolTipText = ViewModel.Title;
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			// 0x0112 == WM_SYSCOMMAND, 'Window' command message.
			// 0xF020 == SC_MINIMIZE, command to minimize the window.
			if (msg == 0x0112 && ((int)wParam & 0xFFF0) == 0xF020)
			{
				// Cancel the minimize.
				handled = true;

				HideToSystemTray();
			}

			return IntPtr.Zero;
		}

		public void HideToSystemTray()
		{
			Hide();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Class is called from XAML")]
		private void DiscArtView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DiscArtView.Image_MouseLeftButtonDown(sender, e);
		}
	}
}
