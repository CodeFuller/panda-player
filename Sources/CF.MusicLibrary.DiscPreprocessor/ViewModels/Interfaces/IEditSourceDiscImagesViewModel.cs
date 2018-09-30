using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditSourceDiscImagesViewModel : IPageViewModel
	{
		ObservableCollection<DiscImageViewItem> ImageItems { get; }

		IEnumerable<AddedDiscImage> AddedImages { get; }

		ICommand RefreshContentCommand { get; }

		void LoadImages(IEnumerable<AddedDisc> discs);
	}
}
