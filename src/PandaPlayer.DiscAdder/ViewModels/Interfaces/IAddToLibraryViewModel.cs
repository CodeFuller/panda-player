using System.Collections.Generic;
using System.Windows.Input;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IAddToLibraryViewModel : IPageViewModel
	{
		bool DeleteSourceContent { get; set; }

		public int CurrentProgress { get; set; }

		public int ProgressMaximum { get; set; }

		public string CurrentProgressPercentage { get; }

		public string ProgressMessages { get; set; }

		public ICommand AddToLibraryCommand { get; }

		void Load(IEnumerable<SongViewItem> songs, IEnumerable<DiscImageViewItem> images);
	}
}
