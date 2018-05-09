using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface ITagDataConsistencyChecker
	{
		Task CheckTagData(IEnumerable<Song> songs, CancellationToken cancellationToken);

		Task UnifyTags(IEnumerable<Song> songs, CancellationToken cancellationToken);
	}
}
