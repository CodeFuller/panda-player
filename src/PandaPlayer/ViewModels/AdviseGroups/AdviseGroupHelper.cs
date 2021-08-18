using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;

namespace PandaPlayer.ViewModels.AdviseGroups
{
	internal class AdviseGroupHelper : IAdviseGroupHelper
	{
		private readonly IAdviseGroupService adviseGroupService;

		public IReadOnlyCollection<AdviseGroupModel> AdviseGroups { get; private set; }

		public AdviseGroupHelper(IAdviseGroupService adviseGroupService)
		{
			this.adviseGroupService = adviseGroupService ?? throw new ArgumentNullException(nameof(adviseGroupService));
		}

		public async Task Load(CancellationToken cancellationToken)
		{
			await UpdateAdviseGroups(cancellationToken);
		}

		public async Task CreateAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, string newAdviseGroupName, CancellationToken cancellationToken)
		{
			var newAdviseGroup = new AdviseGroupModel
			{
				Name = newAdviseGroupName,
			};

			await adviseGroupService.CreateAdviseGroup(newAdviseGroup, cancellationToken);

			await adviseGroupHolder.AssignAdviseGroup(adviseGroupService, newAdviseGroup, cancellationToken);

			await UpdateAdviseGroups(cancellationToken);
		}

		public async Task ReverseAdviseGroup(BasicAdviseGroupHolder adviseGroupHolder, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			if (adviseGroupHolder.CurrentAdviseGroup == null || adviseGroupHolder.CurrentAdviseGroup.Id != adviseGroup.Id)
			{
				await adviseGroupHolder.AssignAdviseGroup(adviseGroupService, adviseGroup, cancellationToken);
			}
			else
			{
				await adviseGroupHolder.RemoveAdviseGroup(adviseGroupService, cancellationToken);
			}

			// Updating advise groups because orphan advise group could be deleted.
			await UpdateAdviseGroups(cancellationToken);
		}

		private async Task UpdateAdviseGroups(CancellationToken cancellationToken)
		{
			AdviseGroups = await adviseGroupService.GetAllAdviseGroups(cancellationToken);
		}
	}
}
