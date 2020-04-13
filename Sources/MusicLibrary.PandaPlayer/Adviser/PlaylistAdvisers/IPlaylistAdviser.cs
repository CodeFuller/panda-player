using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary);
	}
}
