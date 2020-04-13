using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public interface ITagDataConsistencyChecker
	{
		Task CheckTagData(IEnumerable<Song> songs, CancellationToken cancellationToken);

		Task UnifyTags(IEnumerable<Song> songs, CancellationToken cancellationToken);
	}
}
