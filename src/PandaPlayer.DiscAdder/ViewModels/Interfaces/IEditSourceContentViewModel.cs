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

		DiscTreeViewModel ReferenceContent { get; }

		DiscTreeViewModel DiskContent { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		ICommand ReloadReferenceContentCommand { get; }

		ICommand ReloadDiskContentCommand { get; }

		ICommand ReloadAllContentCommand { get; }

		Task LoadDefaultContent(CancellationToken cancellationToken);
	}
}
