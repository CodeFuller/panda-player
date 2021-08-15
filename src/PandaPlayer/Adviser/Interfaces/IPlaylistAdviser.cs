using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IPlaylistAdviser
	{
		Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken);
	}
}
