using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscConsistencyChecker
	{
		Task CheckDiscsConsistency(IEnumerable<Disc> discs);
	}
}
