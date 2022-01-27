using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IAdviseSetRepository
	{
		Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<AdviseSetModel>> GetEmptyAdviseSets(CancellationToken cancellationToken);

		Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);

		Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);
	}
}
