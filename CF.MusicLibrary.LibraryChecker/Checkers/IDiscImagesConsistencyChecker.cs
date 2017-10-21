using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscImagesConsistencyChecker
	{
		Task CheckDiscImagesConsistency(IEnumerable<Disc> discs);
	}
}
