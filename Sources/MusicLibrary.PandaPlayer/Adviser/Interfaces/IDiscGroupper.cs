using System.Collections.Generic;
using MusicLibrary.Core.Models;
using MusicLibrary.PandaPlayer.Adviser.Grouping;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscGroupper
	{
		IEnumerable<DiscGroup> GroupLibraryDiscs(IEnumerable<DiscModel> discs);
	}
}
