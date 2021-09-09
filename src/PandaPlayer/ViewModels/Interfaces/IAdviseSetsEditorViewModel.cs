using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.Core.Models;
using PandaPlayer.ViewModels.AdviseSetsEditor;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IAdviseSetsEditorViewModel
	{
		ObservableCollection<AdviseSetModel> AdviseSets { get; }

		AdviseSetModel SelectedAdviseSet { get; set; }

		bool CanDeleteAdviseSet { get; }

		ObservableCollection<DiscModel> CurrentAdviseSetDiscs { get; }

		DiscModel SelectedAdviseSetDisc { get; set; }

		bool CanAddDisc { get; }

		bool CanRemoveDisc { get; }

		bool CanMoveDiscUp { get; }

		bool CanMoveDiscDown { get; }

		IReadOnlyCollection<AvailableDiscViewModel> AvailableDiscs { get; }

#pragma warning disable CA2227 // Collection properties should be read only
		IList SelectedAvailableDiscItems { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		ICommand CreateAdviseSetCommand { get; }

		ICommand DeleteAdviseSetCommand { get; }

		ICommand AddDiscsCommand { get; }

		ICommand RemoveDiscCommand { get; }

		ICommand MoveDiscUpCommand { get; }

		ICommand MoveDiscDownCommand { get; }

		Task Load(CancellationToken cancellationToken);

		Task RenameAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);
	}
}
