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

		Task SetAdviseSetDiscs(AdviseSetModel adviseSet, IReadOnlyCollection<DiscModel> discs, CancellationToken cancellationToken);

		Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);

		Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken);
	}
}
