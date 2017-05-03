using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	public interface IWindowService
	{
		bool ShowEditAlbumsDetailsWindow(EditAlbumsDetailsViewModel viewModel);

		bool ShowEditSongsDetailsWindow(EditSongsDetailsViewModel viewModel);

		MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
	}
}
