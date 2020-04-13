using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public interface IDiscImagesConsistencyChecker
	{
		Task CheckDiscImagesConsistency(IEnumerable<Disc> discs, CancellationToken cancellationToken);
	}
}
