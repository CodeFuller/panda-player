using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	public abstract class BasicAdviseGroupHolder
	{
		public abstract string InitialAdviseGroupName { get; }

		public abstract AdviseGroupModel CurrentAdviseGroup { get; }

		public abstract Task AssignAdviseGroup(IAdviseGroupService adviseGroupService, AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		public abstract Task RemoveAdviseGroup(IAdviseGroupService adviseGroupService, CancellationToken cancellationToken);
	}
}
