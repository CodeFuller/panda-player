using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Adviser.Grouping;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Adviser.Interfaces
{
	internal interface IDiscGrouper
	{
		Task<IReadOnlyCollection<DiscGroup>> GroupLibraryDiscs(IEnumerable<DiscModel> discs, CancellationToken cancellationToken);
	}
}
