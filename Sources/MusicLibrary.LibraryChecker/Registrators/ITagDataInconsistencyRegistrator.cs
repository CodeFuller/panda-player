using System.Collections.Generic;
using MusicLibrary.Core.Media;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryChecker.Registrators
{
	public interface ITagDataInconsistencyRegistrator
	{
		void RegisterBadTagData(string inconsistencyMessage);

		void RegisterBadTagData(Song song, IEnumerable<SongTagType> tagTypes);
	}
}
