using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	internal class FolderAdviseGroupHolder : BasicAdviseGroupHolder
	{
		private readonly FolderModel folder;

		public override string InitialAdviseGroupName => folder.Name;

		public override AdviseGroupModel CurrentAdviseGroup => folder.AdviseGroup;

		public FolderAdviseGroupHolder(FolderModel folder)
		{
			this.folder = folder ?? throw new ArgumentNullException(nameof(folder));
		}

		public override Task AssignAdviseGroup(IAdviseGroupService adviseGroupService, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			return adviseGroupService.AssignAdviseGroup(folder, adviseGroup, cancellationToken);
		}

		public override Task RemoveAdviseGroup(IAdviseGroupService adviseGroupService, CancellationToken cancellationToken)
		{
			return adviseGroupService.RemoveAdviseGroup(folder, cancellationToken);
		}
	}
}
