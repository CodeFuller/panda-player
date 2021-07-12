using System.Collections.Generic;
using PandaPlayer.Adviser.Internal;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo);
	}
}
