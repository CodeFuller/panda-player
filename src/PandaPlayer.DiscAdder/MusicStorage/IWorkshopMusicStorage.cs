using System.Collections.Generic;

namespace PandaPlayer.DiscAdder.MusicStorage
{
	internal interface IWorkshopMusicStorage
	{
		AddedDiscInfo GetAddedDiscInfo(string sourceDiscPath, IEnumerable<string> songFiles);

		void DeleteSourceContent(IEnumerable<string> contentFiles);
	}
}
