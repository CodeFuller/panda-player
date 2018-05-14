using System.Collections.Generic;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryChecker.Registrators
{
	public interface ITagDataInconsistencyRegistrator
	{
		void RegisterBadTagData(string inconsistencyMessage);

		void RegisterBadTagData(Song song, IEnumerable<SongTagType> tagTypes);
	}
}
