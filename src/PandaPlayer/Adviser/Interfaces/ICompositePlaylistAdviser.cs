using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface ICompositePlaylistAdviser
	{
		Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, int requiredAdvisesCount, CancellationToken cancellationToken);

		Task RegisterAdvicePlayback(AdvisedPlaylist advise, CancellationToken cancellationToken);
	}
}
