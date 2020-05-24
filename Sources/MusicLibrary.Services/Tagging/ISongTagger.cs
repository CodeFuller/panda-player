using System.Collections.Generic;

namespace MusicLibrary.Services.Tagging
{
	public interface ISongTagger
	{
		void SetTagData(string songFileName, SongTagData tagData);

		SongTagData GetTagData(string songFileName);

		IEnumerable<SongTagType> GetTagTypes(string songFileName);

		void FixTagData(string songFileName);
	}
}
