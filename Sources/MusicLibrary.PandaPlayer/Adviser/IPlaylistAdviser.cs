using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser
{
	public interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise();
	}
}
