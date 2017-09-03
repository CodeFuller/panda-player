using System.Collections.Generic;

namespace CF.MusicLibrary.Tagger
{
	public interface ISongTagger
	{
		void SetTagData(string songFileName, SongTagData tagData);

		SongTagData GetTagData(string songFileName);

		IEnumerable<AudioTagType> GetTagTypes(string songFileName);

		void FixTagData(string songFileName);
	}
}
