using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MusicLibrary.DiscAdder.AddingToLibrary;
using MusicLibrary.DiscAdder.MusicStorage;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditDiscsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<DiscViewItem> Discs { get; }

		IEnumerable<AddedDisc> AddedDiscs { get; }

		IEnumerable<AddedSong> AddedSongs { get; }

		Task SetDiscs(IEnumerable<AddedDiscInfo> discs);
	}
}
