using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.DiscAdder.MusicStorage;
using PandaPlayer.DiscAdder.ViewModels.SourceContent;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	internal interface IEditSourceContentViewModel : IPageViewModel
	{
		DiscTreeViewModel CurrentDiscs { get; }

		IEnumerable<AddedDiscInfo> AddedDiscs { get; }

		Task LoadDefaultContent(CancellationToken cancellationToken);
	}
}
