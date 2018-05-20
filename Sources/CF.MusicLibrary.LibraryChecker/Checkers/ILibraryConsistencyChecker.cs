using System.Threading;
using System.Threading.Tasks;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface ILibraryConsistencyChecker
	{
		Task CheckLibrary(LibraryCheckFlags checkFlags, bool fixIssues, CancellationToken cancellationToken);

		Task UnifyTags(CancellationToken cancellationToken);
	}
}
