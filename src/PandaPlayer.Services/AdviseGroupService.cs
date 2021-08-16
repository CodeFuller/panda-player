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

		public AdviseGroupService(IAdviseGroupRepository adviseGroupRepository, IFoldersRepository foldersRepository)
		{
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
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

		public async Task AssignAdviseGroup(ShallowFolderModel folder, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			folder.AdviseGroup = adviseGroup;

			await foldersRepository.UpdateFolder(folder, cancellationToken);
		}

		public async Task RemoveAdviseGroup(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			folder.AdviseGroup = null;

			await foldersRepository.UpdateFolder(folder, cancellationToken);
		}
	}
}
