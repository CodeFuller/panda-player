using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface ICompositePlaylistAdviser
	{
		IEnumerable<AdvisedPlaylist> Advise(IEnumerable<DiscModel> discs);

		void RegisterAdvicePlayback(AdvisedPlaylist advise);
	}
}
