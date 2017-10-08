using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.PlaylistAdvisers
{
	public interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise(DiscLibrary discLibrary);
	}
}
