using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Internal;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs, PlaybacksInfo playbacksInfo);
	}
}
