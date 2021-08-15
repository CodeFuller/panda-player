using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IAdviseGroupRepository
	{
		Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups(CancellationToken cancellationToken);
	}
}
