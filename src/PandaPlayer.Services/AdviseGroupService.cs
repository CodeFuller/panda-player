using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Comparers;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class AdviseGroupService : IAdviseGroupService
	{
		private readonly IAdviseGroupRepository adviseGroupRepository;

		private readonly IFoldersRepository foldersRepository;

		private readonly IDiscsRepository discsRepository;

		private readonly ILogger<AdviseGroupService> logger;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public AdviseGroupService(IAdviseGroupRepository adviseGroupRepository, IFoldersRepository foldersRepository,
			IDiscsRepository discsRepository, ILogger<AdviseGroupService> logger)
		{
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			await adviseGroupRepository.CreateAdviseGroup(adviseGroup, cancellationToken);

			DiscLibrary.AddAdviseGroup(adviseGroup);
		}

		public Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups(CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.AdviseGroups);
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
				await DeleteOrphanAdviseGroups(cancellationToken);
			}
		}

		public async Task AssignAdviseGroup(DiscModel disc, AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			var hasAdviseGroup = disc.AdviseGroup != null;

			disc.AdviseGroup = adviseGroup;
			await discsRepository.UpdateDisc(disc, cancellationToken);

			if (hasAdviseGroup)
			{
				await DeleteOrphanAdviseGroups(cancellationToken);
			}
		}

		public async Task RemoveAdviseGroup(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			folder.AdviseGroup = null;

			await foldersRepository.UpdateFolder(folder, cancellationToken);

			await DeleteOrphanAdviseGroups(cancellationToken);
		}

		public async Task RemoveAdviseGroup(DiscModel disc, CancellationToken cancellationToken)
		{
			disc.AdviseGroup = null;

			await discsRepository.UpdateDisc(disc, cancellationToken);

			await DeleteOrphanAdviseGroups(cancellationToken);
		}

		public async Task DeleteOrphanAdviseGroups(CancellationToken cancellationToken)
		{
			var folderAdviseGroups = DiscLibrary.Folders.Select(x => x.AdviseGroup).Where(x => x != null);
			var discAdviseGroups = DiscLibrary.Discs.Select(x => x.AdviseGroup).Where(x => x != null);

			var activeAdviseGroups = folderAdviseGroups.Concat(discAdviseGroups).Distinct(new AdviseGroupEqualityComparer());
			var orphanAdviseGroups = DiscLibrary.AdviseGroups.Except(activeAdviseGroups, new AdviseGroupEqualityComparer()).ToList();

			foreach (var orphanAdviseGroup in orphanAdviseGroups)
			{
				logger.LogInformation($"Deleting advise group '{orphanAdviseGroup.Name}' ...");
				await adviseGroupRepository.DeleteAdviseGroup(orphanAdviseGroup, cancellationToken);

				DiscLibrary.DeleteAdviseGroup(orphanAdviseGroup);
			}
		}
	}
}
