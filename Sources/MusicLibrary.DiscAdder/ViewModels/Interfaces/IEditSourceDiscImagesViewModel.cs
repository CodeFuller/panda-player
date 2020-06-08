using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSourceDiscImagesViewModel : IPageViewModel
	{
		ObservableCollection<DiscImageViewItem> ImageItems { get; }

		IEnumerable<AddedDiscImage> AddedImages { get; }

		ICommand RefreshContentCommand { get; }

		void LoadImages(IEnumerable<AddedDisc> discs);
	}
}
