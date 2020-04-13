using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MusicLibrary.DiscPreprocessor.AddingToLibrary;
using MusicLibrary.DiscPreprocessor.MusicStorage;

namespace MusicLibrary.DiscPreprocessor.ViewModels.Interfaces
{
	public interface IEditDiscsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<DiscViewItem> Discs { get; }

		IEnumerable<AddedDisc> AddedDiscs { get; }

		IEnumerable<AddedSong> AddedSongs { get; }

		Task SetDiscs(IEnumerable<AddedDiscInfo> discs);
	}
}
