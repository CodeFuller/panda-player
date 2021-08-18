using System;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	internal class DiscAdviseGroupHolder : BasicAdviseGroupHolder
	{
		private readonly DiscModel disc;

		public override string InitialAdviseGroupName => disc.AlbumTitle ?? disc.Title;

		public override AdviseGroupModel CurrentAdviseGroup => disc.AdviseGroup;

		public DiscAdviseGroupHolder(DiscModel disc)
		{
			this.disc = disc ?? throw new ArgumentNullException(nameof(disc));
		}

		public override Task AssignAdviseGroup(IAdviseGroupService adviseGroupService, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			return adviseGroupService.AssignAdviseGroup(disc, adviseGroup, cancellationToken);
		}

		public override Task RemoveAdviseGroup(IAdviseGroupService adviseGroupService, CancellationToken cancellationToken)
		{
			return adviseGroupService.RemoveAdviseGroup(disc, cancellationToken);
		}
	}
}
