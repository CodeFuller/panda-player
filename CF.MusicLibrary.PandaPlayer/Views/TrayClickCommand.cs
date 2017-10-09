using System;
using System.Windows;
using System.Windows.Input;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used in window XAML.")]
	internal class TrayClickCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			if (parameter.ToString() == "LeftClick")
			{
				Messenger.Default.Send(new ReversePlayingEventArgs());
			}
			else if (parameter.ToString() == "DoubleClick")
			{
				Application.Current.MainWindow.Show();
				Application.Current.MainWindow.Activate();
			}
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		protected virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
