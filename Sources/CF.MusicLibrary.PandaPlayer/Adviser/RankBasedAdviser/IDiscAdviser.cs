using System.Collections.Generic;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	public interface IDiscAdviser
	{
		IEnumerable<Disc> AdviseDiscs(DiscLibrary discLibrary);
	}
}
