using System.Collections.Generic;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface ITagDataConsistencyChecker
	{
		void CheckTagData(IEnumerable<Song> songs);

		void UnifyTags(IEnumerable<Song> songs);
	}
}
