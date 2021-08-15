using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
{
	internal class AdviseGroupService : IAdviseGroupService
	{
		private readonly IAdviseGroupRepository adviseGroupRepository;

		public AdviseGroupService(IAdviseGroupRepository adviseGroupRepository)
		{
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
		}

		public Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			return adviseGroupRepository.CreateAdviseGroup(adviseGroup, cancellationToken);
		}

		public async Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups(CancellationToken cancellationToken)
		{
			return (await adviseGroupRepository.GetAllAdviseGroups(cancellationToken))
				.OrderBy(ag => ag.Name)
				.ToList();
		}
	}
}
