using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MusicLibrary.DiscAdder.AddedContent;
using MusicLibrary.DiscAdder.ViewModels.ViewModelItems;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceDiscImagesViewModel : IPageViewModel
	{
		ObservableCollection<DiscImageViewItem> ImageItems { get; }

		IEnumerable<AddedDiscImage> AddedImages { get; }

		ICommand RefreshContentCommand { get; }

		void LoadImages(IEnumerable<AddedDisc> discs);
	}
}
