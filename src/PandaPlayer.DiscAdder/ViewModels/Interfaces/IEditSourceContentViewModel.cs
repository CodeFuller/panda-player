using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceContentViewModel : IPageViewModel
	{
		IReferenceContentViewModel RawReferenceDiscs { get; }

		DiscTreeViewModel ReferenceDiscs { get; }

		DiscTreeViewModel CurrentDiscs { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		ICommand ReloadRawContentCommand { get; }

		Task LoadDefaultContent(CancellationToken cancellationToken);
	}
}
