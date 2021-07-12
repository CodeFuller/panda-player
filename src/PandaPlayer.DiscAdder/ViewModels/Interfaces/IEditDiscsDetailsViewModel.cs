using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.DiscAdder.AddedContent;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.ViewModelItems;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditDiscsDetailsViewModel : IPageViewModel
	{
		ObservableCollection<DiscViewItem> Discs { get; }

		IEnumerable<AddedDisc> AddedDiscs { get; }

		IEnumerable<AddedSong> AddedSongs { get; }

		Task SetDiscs(IEnumerable<AddedDiscInfo> discs, CancellationToken cancellationToken);
	}
}
