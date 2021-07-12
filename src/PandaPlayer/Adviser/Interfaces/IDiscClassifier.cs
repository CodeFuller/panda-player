using System.Collections.Generic;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscClassifier
	{
		IEnumerable<DiscGroup> GroupLibraryDiscs(IEnumerable<DiscModel> discs);
	}
}
