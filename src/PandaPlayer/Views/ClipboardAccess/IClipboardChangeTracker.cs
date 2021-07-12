using System;

namespace PandaPlayer.Views.ClipboardAccess
{
	public interface IClipboardChangeTracker
	{
		event EventHandler<ClipboardContentChangedEventArgs> ClipboardContentChanged;

		void StartTracking();

		void StopTracking();
	}
}
