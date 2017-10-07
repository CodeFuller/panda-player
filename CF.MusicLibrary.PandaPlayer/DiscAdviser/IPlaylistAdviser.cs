using System.Collections.ObjectModel;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public interface IPlaylistAdviser
	{
		Collection<AdvisedPlaylist> Advise(DiscLibrary discLibrary);
	}
}
