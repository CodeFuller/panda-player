using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	public interface IAdviseGroupHelper
	{
		IReadOnlyCollection<AdviseGroupModel> AdviseGroups { get; }

		Task Load(CancellationToken cancellationToken);

		Task CreateAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, string newAdviseGroupName, CancellationToken cancellationToken);

		Task ReverseAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		Task ReverseFavoriteStatus(AdviseGroupModel adviseGroup, CancellationToken cancellationToken);
	}
}
