using System.Collections.Generic;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscAdviser
	{
		IEnumerable<DiscModel> AdviseDiscs(IEnumerable<DiscModel> discs);
	}
}
