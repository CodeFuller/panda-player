using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface ILibraryCheckerViewModel
	{
		bool CheckDiscsConsistency { get; set; }

		bool CheckStorageConsistency { get; set; }

		bool CheckContentConsistency { get; set; }

		bool CheckTagsConsistency { get; set; }

		bool IsRunning { get; }

		double CheckProgressMaximum { get; set; }

		double CheckProgressValue { get; set; }

		string CheckProgressPercentage { get; }

		ICommand RunCheckCommand { get; }

		public ObservableCollection<DiagnosticInconsistencyViewModel> Inconsistencies { get; }
	}
}
