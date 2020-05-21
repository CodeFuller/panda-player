using System.Collections.Generic;
using System.Linq;

namespace MusicLibrary.PandaPlayer.Adviser
{
	// TBD: Remove after redesign
	internal class StubPlaylistAdviser : ICompositePlaylistAdviser
	{
		public IEnumerable<AdvisedPlaylist> Advise()
		{
			return Enumerable.Empty<AdvisedPlaylist>();
		}

		public void RegisterAdvicePlayback(AdvisedPlaylist advise)
		{
		}
	}
}
