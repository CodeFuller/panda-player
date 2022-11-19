using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IAdviseSetService
	{
		Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<AdviseSetModel>> GetAllAdviseSets(CancellationToken cancellationToken);

		Task AddDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> addedDiscs, CancellationToken cancellationToken);

		Task RemoveDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> removedDiscs, CancellationToken cancellationToken);

		Task ReorderDiscs(AdviseSetModel adviseSet, IEnumerable<DiscModel> newDiscsOrder, CancellationToken cancellationToken);

		Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);

		Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);
	}
}
