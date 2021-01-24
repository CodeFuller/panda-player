using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using MusicLibrary.PandaPlayer.Views.ClipboardAccess;

namespace MusicLibrary.PandaPlayer.Views
{
	public partial class EditDiscImageView : Window
	{
		private IEditDiscImageViewModel ViewModel => DataContext.GetViewModel<IEditDiscImageViewModel>();

		private readonly IClipboardChangeTracker clipboardChangeTracker = new ClipboardChangeTracker();

		private readonly IClipboardDataProvider clipboardDataProvider = new ClipboardDataProvider();

		public EditDiscImageView()
		{
			InitializeComponent();
		}

		private void ClipboardChangeTrackerOnClipboardContentChanged(object sender, ClipboardContentChangedEventArgs clipboardContentChangedEventArgs)
		{
			string textData = clipboardDataProvider.GetTextData();
			if (textData != null)
			{
				if (Uri.TryCreate(textData, UriKind.Absolute, out var imageUri))
				{
					ViewModel.SetImage(imageUri);
				}

				return;
			}

			BitmapFrame imageData = clipboardDataProvider.GetImageData();
			if (imageData != null)
			{
				var encoder = new JpegBitmapEncoder();
				encoder.Frames.Add(imageData);

				using (var memoryStream = new MemoryStream())
				{
					encoder.Save(memoryStream);
					ViewModel.SetImage(memoryStream.ToArray());
				}
			}
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			await ViewModel.Save(CancellationToken.None);
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			clipboardChangeTracker.ClipboardContentChanged += ClipboardChangeTrackerOnClipboardContentChanged;
			clipboardChangeTracker.StartTracking();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			clipboardChangeTracker.ClipboardContentChanged -= ClipboardChangeTrackerOnClipboardContentChanged;
			clipboardChangeTracker.StopTracking();

			base.OnClosing(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			ViewModel.Unload();

			base.OnClosed(e);
		}
	}
}
