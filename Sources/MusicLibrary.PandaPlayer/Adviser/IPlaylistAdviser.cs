using System.Collections.Generic;

namespace MusicLibrary.PandaPlayer.Adviser
{
	public interface IPlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise();
	}
}
