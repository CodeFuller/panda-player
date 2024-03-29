using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IAdviseGroupService
	{
		Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups(CancellationToken cancellationToken);

		Task UpdateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task AssignAdviseGroup(FolderModel folder, AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task AssignAdviseGroup(DiscModel disc, AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task RemoveAdviseGroup(FolderModel folder, CancellationToken cancellationToken);

		Task RemoveAdviseGroup(DiscModel disc, CancellationToken cancellationToken);

		Task DeleteOrphanAdviseGroups(CancellationToken cancellationToken);
	}
}
