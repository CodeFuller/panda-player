using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface ITagDataConsistencyChecker
	{
		Task CheckTagData(IEnumerable<Song> songs);

		Task UnifyTags(IEnumerable<Song> songs);
	}
}
