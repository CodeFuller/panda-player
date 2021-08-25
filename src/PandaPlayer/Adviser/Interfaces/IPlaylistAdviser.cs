using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Adviser.Internal;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IPlaylistAdviser
	{
		Task<IReadOnlyCollection<AdvisedPlaylist>> Advise(IEnumerable<AdviseGroupContent> adviseGroups, PlaybacksInfo playbacksInfo, CancellationToken cancellationToken);
	}
}
