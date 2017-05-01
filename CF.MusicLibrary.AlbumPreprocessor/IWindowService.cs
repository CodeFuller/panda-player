using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public interface IWindowService
	{
		bool ShowAddToLibraryWindow(AddToLibraryViewModel viewModel);

		MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
	}
}
