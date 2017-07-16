using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels.Interfaces;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public interface IWindowService
	{
		bool ShowEditAlbumsDetailsWindow(IEditAlbumsDetailsViewModel viewModel);

		bool ShowEditSongsDetailsWindow(IEditSongsDetailsViewModel viewModel);

		MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
	}
}
