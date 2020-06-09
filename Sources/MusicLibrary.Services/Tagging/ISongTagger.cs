using System.Collections.Generic;

namespace MusicLibrary.Services.Tagging
{
	public interface ISongTagger
	{
		void SetTagData(string songFileName, SongTagData tagData);

		// TODO: Remove these methods or restore consistency check for tags data.
		// TODO: If these methods are removed, then ISongMediaInfoProvider can be moved to DiscAdder project.
		SongTagData GetTagData(string songFileName);

		IEnumerable<SongTagType> GetTagTypes(string songFileName);

		void FixTagData(string songFileName);
	}
}
