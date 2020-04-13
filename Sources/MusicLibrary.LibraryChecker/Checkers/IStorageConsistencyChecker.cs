using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public interface IStorageConsistencyChecker
	{
		Task CheckStorage(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken);

		Task CheckStorageChecksums(DiscLibrary library, bool fixIssues, CancellationToken cancellationToken);
	}
}
