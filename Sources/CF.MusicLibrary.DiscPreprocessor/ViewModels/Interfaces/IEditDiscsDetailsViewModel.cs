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

		IEnumerable<AddedDisc> AddedDiscs { get; }

		IEnumerable<AddedSong> AddedSongs { get; }

		Task SetDiscs(IEnumerable<AddedDiscInfo> discs);
	}
}
