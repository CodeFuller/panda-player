using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.DiscAdder.MusicStorage;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceContentViewModel : IPageViewModel
	{
		IRawReferenceContentViewModel RawReferenceContent { get; }

		IReferenceContentViewModel ReferenceContent { get; }

		IActualContentViewModel ActualContent { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		ICommand ReloadReferenceContentCommand { get; }

		ICommand ReloadActualContentCommand { get; }

		ICommand ReloadAllContentCommand { get; }

		Task Load(CancellationToken cancellationToken);

		Task ResetContent(CancellationToken cancellationToken);
	}
}
