using System.Collections.Generic;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.PandaPlayer.Adviser.RankBasedAdviser
{
	public interface IDiscAdviser
	{
		IEnumerable<Disc> AdviseDiscs(DiscLibrary discLibrary);
	}
}
