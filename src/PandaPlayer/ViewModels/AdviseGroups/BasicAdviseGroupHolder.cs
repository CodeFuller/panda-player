using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	internal abstract class BasicAdviseGroupHolder
	{
		public abstract string InitialAdviseGroupName { get; }

		public abstract AdviseGroupModel AdviseGroup { get; }

		public abstract Task AssignAdviseGroup(IAdviseGroupService adviseGroupService, AdviseGroupModel adviseGroup, CancellationToken cancellationToken);

		public abstract Task RemoveAdviseGroup(IAdviseGroupService adviseGroupService, CancellationToken cancellationToken);
	}
}
