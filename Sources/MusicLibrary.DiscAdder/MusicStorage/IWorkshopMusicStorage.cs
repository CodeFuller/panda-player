using System.Collections.Generic;

namespace MusicLibrary.DiscAdder.MusicStorage
{
	public interface IWorkshopMusicStorage
	{
		AddedDiscInfo GetAddedDiscInfo(string sourceDiscPath, IEnumerable<string> songFiles);

		void DeleteSourceContent(IEnumerable<string> contentFiles);
	}
}
