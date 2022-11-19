using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	public interface IDiscAdderViewModel
	{
		event EventHandler OnRequestClose;

		bool CanSwitchToPrevPage { get; }

		bool CanSwitchToNextPage { get; }

		string PrevPageName { get; }

		string NextPageName { get; }

		IPageViewModel CurrentPage { get; set; }

		ICommand SwitchToPrevPageCommand { get; }

		ICommand SwitchToNextPageCommand { get; }

		Task Load(CancellationToken cancellationToken);
	}
}
