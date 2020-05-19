using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser
{
	// TBD: Remove after redesign
	internal class StubPlaylistAdviser : ICompositePlaylistAdviser
	{
		private readonly DiscLibrary discLibrary;

		public StubPlaylistAdviser(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public IEnumerable<AdvisedPlaylist> Advise()
		{
			return new[] { AdvisedPlaylist.ForDisc(discLibrary.Discs.First()) };
		}

		public void RegisterAdvicePlayback(AdvisedPlaylist advise)
		{
		}
	}
}
