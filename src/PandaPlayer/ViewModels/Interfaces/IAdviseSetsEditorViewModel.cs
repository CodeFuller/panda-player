using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IAdviseSetsEditorViewModel
	{
		ObservableCollection<AdviseSetModel> AdviseSets { get; }

		AdviseSetModel SelectedAdviseSet { get; set; }

		bool CanDeleteAdviseSet { get; }

		ObservableCollection<DiscModel> CurrentAdviseSetDiscs { get; }

		DiscModel SelectedAdviseSetDisc { get; set; }

		bool CanAddDiscs { get; }

		bool CanRemoveDisc { get; }

		bool CanMoveDiscUp { get; }

		bool CanMoveDiscDown { get; }

		IAvailableDiscsViewModel AvailableDiscsViewModel { get; }

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
