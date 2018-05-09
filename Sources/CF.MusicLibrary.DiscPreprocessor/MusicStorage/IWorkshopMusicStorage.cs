using System.Collections.Generic;

namespace CF.MusicLibrary.DiscPreprocessor.MusicStorage
{
	public interface IWorkshopMusicStorage
	{
		AddedDiscInfo GetAddedDiscInfo(string discPath, IEnumerable<string> songFiles);

		void DeleteSourceContent(IEnumerable<string> contentFiles);
	}
}
