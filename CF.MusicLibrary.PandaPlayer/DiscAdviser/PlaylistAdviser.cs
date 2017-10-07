using System;
using System.Collections.ObjectModel;
using System.Linq;
using CF.Library.Core.Extensions;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.DiscAdviser
{
	public class PlaylistAdviser : IPlaylistAdviser
	{
		private readonly IDiscAdviser discAdviser;

		public PlaylistAdviser(IDiscAdviser discAdviser)
		{
			if (discAdviser == null)
			{
				throw new ArgumentNullException(nameof(discAdviser));
			}

			this.discAdviser = discAdviser;
		}

		public Collection<AdvisedPlaylist> Advise(DiscLibrary discLibrary)
		{
			return discAdviser.AdviseDiscs(discLibrary).Select(d => new AdvisedPlaylist(d)).ToCollection();
		}
	}
}
