using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class FoldersService : IFoldersService
	{
		private readonly IFoldersRepository foldersRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IAdviseGroupService adviseGroupService;

		private readonly IClock clock;

		private readonly ILogger<FoldersService> logger;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public FoldersService(IFoldersRepository foldersRepository, IStorageRepository storageRepository,
			IAdviseGroupService adviseGroupService, IClock clock, ILogger<FoldersService> logger)
		{
			this.foldersRepository = foldersRepository ?? throw new ArgumentNullException(nameof(foldersRepository));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.adviseGroupService = adviseGroupService ?? throw new ArgumentNullException(nameof(adviseGroupService));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateEmptyFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			await storageRepository.CreateFolder(folder, cancellationToken);

			await foldersRepository.CreateEmptyFolder(folder, cancellationToken);

			DiscLibrary.AddFolder(folder);
		}

		public Task<FolderModel> GetRootFolder(CancellationToken cancellationToken)
		{
			var rootFolder = DiscLibrary.Folders.Single(x => x.IsRoot);
			return Task.FromResult(rootFolder);
		}

		public async Task UpdateFolder(FolderModel folder, Action<FolderModel> updateAction, CancellationToken cancellationToken)
		{
			var currentFolder = folder.CloneShallow();

			updateAction(folder);

			if (!folder.IsDeleted && folder.Name != currentFolder.Name)
			{
				await storageRepository.RenameFolder(currentFolder, folder, cancellationToken);
			}

			await foldersRepository.UpdateFolder(folder, cancellationToken);
		}

		public async Task DeleteEmptyFolder(FolderModel folder, CancellationToken cancellationToken)
		{
			if (folder.HasContent)
			{
				throw new InvalidOperationException($"Can not delete non-empty folder '{folder.Name}'");
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
				await adviseGroupService.DeleteOrphanAdviseGroups(cancellationToken);
			}

			logger.LogInformation($"The folder '{folder.Name}' was deleted successfully");
		}
	}
}
