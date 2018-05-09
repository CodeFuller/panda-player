using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscConsistencyChecker
	{
		Task CheckDiscsConsistency(IEnumerable<Disc> discs, CancellationToken cancellationToken);
	}
}
