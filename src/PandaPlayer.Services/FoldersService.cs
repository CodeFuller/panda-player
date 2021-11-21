using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
{
	internal class FoldersService : IFoldersService
	{
		private readonly IFoldersRepository foldersRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IAdviseGroupRepository adviseGroupRepository;

		private readonly IClock clock;

		private readonly ILogger<FoldersService> logger;

		public FoldersService(IFoldersRepository foldersRepository, IStorageRepository storageRepository,
			IAdviseGroupRepository adviseGroupRepository, IClock clock, ILogger<FoldersService> logger)
		{
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			await storageRepository.CreateFolder(folder, cancellationToken);

			await foldersRepository.CreateFolder(folder, cancellationToken);
		}

		public Task<IReadOnlyCollection<ShallowFolderModel>> GetAllFolders(CancellationToken cancellationToken)
		{
			return foldersRepository.GetAllFolders(cancellationToken);
		}

		public Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			return foldersRepository.GetRootFolder(cancellationToken);
		}

		public Task<FolderModel> GetFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			return foldersRepository.GetFolder(folderId, cancellationToken);
		}

		public async Task UpdateFolder(ShallowFolderModel folder, CancellationToken cancellationToken)
		{
			var currentFolder = await foldersRepository.GetFolder(folder.Id, cancellationToken);

			if (!folder.IsDeleted && folder.Name != currentFolder.Name)
			{
				await storageRepository.RenameFolder(currentFolder, folder, cancellationToken);
			}

			await foldersRepository.UpdateFolder(folder, cancellationToken);
		}

		public async Task DeleteFolder(ItemId folderId, CancellationToken cancellationToken)
		{
			var folder = await foldersRepository.GetFolder(folderId, cancellationToken);
			if (folder.HasContent)
			{
				throw new InvalidOperationException($"Can not delete non-empty directory '{folder.Name}'");
			}

			logger.LogInformation($"Deleting folder '{folder.Name}' ...");

			await storageRepository.DeleteFolder(folder, cancellationToken);

			var hasAdviseGroup = folder.AdviseGroup != null;
			if (hasAdviseGroup)
			{
				// We erase advise group so that it could be deleted when no references left.
				folder.AdviseGroup = null;
			}

			folder.DeleteDate = clock.Now;
			await foldersRepository.UpdateFolder(folder, cancellationToken);

			if (hasAdviseGroup)
			{
				await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
			}

			logger.LogInformation($"The folder '{folder.Name}' was deleted successfully");
		}
	}
}
