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

		private readonly IFoldersRepository foldersRepository;

		private readonly IDiscsRepository discsRepository;

		public AdviseGroupService(IAdviseGroupRepository adviseGroupRepository, IFoldersRepository foldersRepository, IDiscsRepository discsRepository)
		{
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
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

		public Task UpdateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			return adviseGroupRepository.UpdateAdviseGroup(adviseGroup, cancellationToken);
		}

		public async Task AssignAdviseGroup(ShallowFolderModel folder, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			var hasAdviseGroup = folder.AdviseGroup != null;

			folder.AdviseGroup = adviseGroup;
			await foldersRepository.UpdateFolder(folder, cancellationToken);

			if (hasAdviseGroup)
			{
				await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
			}
		}

		public async Task AssignAdviseGroup(DiscModel disc, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			var hasAdviseGroup = disc.AdviseGroup != null;

			disc.AdviseGroup = adviseGroup;
			await discsRepository.UpdateDisc(disc, cancellationToken);

			if (hasAdviseGroup)
			{
				await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
			}
		}

		public async Task RemoveAdviseGroup(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			folder.AdviseGroup = null;

			await foldersRepository.UpdateFolder(folder, cancellationToken);

			await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
		}

		public async Task RemoveAdviseGroup(DiscModel disc, CancellationToken cancellationToken)
		{
			disc.AdviseGroup = null;

			await discsRepository.UpdateDisc(disc, cancellationToken);

			await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
		}
	}
}
