using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditDiscsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<DiscViewItem> Discs { get; }

		IEnumerable<AddedSong> AddedSongs { get; }

		IEnumerable<AddedDiscCoverImage> DiscCoverImages { get; }

		Task SetDiscs(IEnumerable<AddedDiscInfo> discs);
	}
}
