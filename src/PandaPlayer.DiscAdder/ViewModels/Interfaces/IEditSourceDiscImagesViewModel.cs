using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceDiscImagesViewModel : IPageViewModel
	{
		ObservableCollection<DiscImageViewItem> ImageItems { get; }

		ICommand RefreshContentCommand { get; }

		void Load(IEnumerable<DiscViewItem> discs);
	}
}
