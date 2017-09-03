using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscConsistencyChecker
	{
		void CheckDiscsConsistency(IEnumerable<Disc> discs);
	}
}
