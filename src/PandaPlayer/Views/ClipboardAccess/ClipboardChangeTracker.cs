using System;
using System.Windows;
using System.Windows.Interop;

namespace PandaPlayer.Views.ClipboardAccess
{
	// https://blogs.msdn.microsoft.com/codefx/2012/03/07/sample-of-mar-7th-monitor-windows-clipboard-changes-in-wpf/
	internal class ClipboardChangeTracker : Window, IClipboardChangeTracker
	{
		private bool isStarted;

		private IntPtr nextClipboardViewer;

		private HwndSource hwndSource;

		public event EventHandler<ClipboardContentChangedEventArgs> ClipboardContentChanged;

		public void StartTracking()
		{
			if (isStarted)
			{
				throw new InvalidOperationException("Clipboard tracker is already started");
			}

			IntPtr handle = new WindowInteropHelper(this).EnsureHandle();
			if (handle == IntPtr.Zero)
			{
				throw new InvalidOperationException("Handle of clipboard notification window is null");
			}

			hwndSource = HwndSource.FromHwnd(handle);
			if (hwndSource == null)
			{
				throw new InvalidOperationException("HwndSource of clipboard notification window is null");
			}

			hwndSource.AddHook(WinProc);
			nextClipboardViewer = NativeMethods.SetClipboardViewer(hwndSource.Handle);

			isStarted = true;
		}

		public void StopTracking()
		{
			if (!isStarted)
			{
				throw new InvalidOperationException("Clipboard tracker is not started");
			}

			RemoveClipboardHook();
			isStarted = false;

			// Without this call Application process will not close properly because background Window still exist.
			Close();
		}

		protected virtual void OnClipboardContentChanged(ClipboardContentChangedEventArgs e)
		{
			if (isStarted)
			{
				ClipboardContentChanged?.Invoke(this, e);
			}
		}

		private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case NativeMethods.WM_CHANGECBCHAIN:
					if (wParam == nextClipboardViewer)
					{
						// Clipboard viewer chain changed, need to fix it.
						nextClipboardViewer = lParam;
					}
					else if (nextClipboardViewer != IntPtr.Zero)
					{
						// Pass the message to the next viewer.
						NativeMethods.SendMessage(nextClipboardViewer, msg, wParam, lParam);
					}

					break;

				case NativeMethods.WM_DRAWCLIPBOARD:
					OnClipboardContentChanged(new ClipboardContentChangedEventArgs());

					// Pass the message to the next viewer.
					NativeMethods.SendMessage(nextClipboardViewer, msg, wParam, lParam);
					break;
			}

			return IntPtr.Zero;
		}

		private void RemoveClipboardHook()
		{
			if (hwndSource != null)
			{
				NativeMethods.ChangeClipboardChain(hwndSource.Handle, nextClipboardViewer);
				nextClipboardViewer = IntPtr.Zero;
				hwndSource.RemoveHook(WinProc);
				hwndSource = null;
			}
		}
	}
}
