using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseSetsEditor;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IAvailableDiscsViewModel : INotifyPropertyChanged
	{
		ObservableCollection<AvailableDiscViewModel> AvailableDiscs { get; }

#pragma warning disable CA2227 // Collection properties should be read only
		IList SelectedItems { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		IEnumerable<DiscModel> SelectedDiscs { get; }

		Task LoadDiscs(IEnumerable<DiscModel> activeLibraryDiscs, CancellationToken cancellationToken);

		void LoadAvailableDiscsForAdviseSet(IReadOnlyCollection<DiscModel> adviseSetDiscs);
	}
}
