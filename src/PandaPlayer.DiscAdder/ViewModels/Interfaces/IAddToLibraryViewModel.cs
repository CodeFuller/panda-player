using System.Collections.Generic;
using System.Windows.Input;
using PandaPlayer.DiscAdder.AddedContent;

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

		void SetSongs(IEnumerable<AddedSong> songs);

		void SetDiscsImages(IEnumerable<AddedDiscImage> images);
	}
}
