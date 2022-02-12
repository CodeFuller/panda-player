using System.Collections.Generic;
using System.Collections.ObjectModel;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSongsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<SongViewItem> Songs { get; }

		void Load(IEnumerable<DiscViewItem> discs);
	}
}
