using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscArtConsistencyChecker
	{
		Task CheckDiscArtsConsistency(IEnumerable<Disc> discs);
	}
}
