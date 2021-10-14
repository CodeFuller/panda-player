using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Facades;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
{
	internal class DiscsService : IDiscsService
	{
		private readonly IDiscsRepository discsRepository;

		private readonly ISongsService songsService;

		private readonly IStorageRepository storageRepository;

		private readonly IAdviseGroupRepository adviseGroupRepository;

		private readonly IClock clock;

		private readonly ILogger<DiscsService> logger;

		public DiscsService(IDiscsRepository discsRepository, ISongsService songsService, IStorageRepository storageRepository,
			IAdviseGroupRepository adviseGroupRepository, IClock clock, ILogger<DiscsService> logger)
		{
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.adviseGroupRepository = adviseGroupRepository ?? throw new ArgumentNullException(nameof(adviseGroupRepository));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await storageRepository.CreateDisc(disc, cancellationToken);

			await discsRepository.CreateDisc(disc, cancellationToken);
		}

		public Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken)
		{
			return discsRepository.GetAllDiscs(cancellationToken);
		}

		public async Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			// Reading current disc properties for several reasons:
			// 1. We need to understand which properties were actually changed.
			//    It defines whether song content will be updated (for correct tag values) and whether disc folder in the storage must be renamed.
			// 2. Avoid overwriting of changes made by another clients.
			var currentDisc = await discsRepository.GetDisc(disc.Id, cancellationToken);

			if (disc.TreeTitle != currentDisc.TreeTitle)
			{
				await storageRepository.UpdateDiscTreeTitle(currentDisc, disc, cancellationToken);
			}

			// Checking if we should update storage data (tags) for disc songs.
			if (disc.AlbumTitle != currentDisc.AlbumTitle || disc.Year != currentDisc.Year)
			{
				foreach (var song in disc.ActiveSongs)
				{
					await songsService.UpdateSong(song, cancellationToken);
				}
			}

			await discsRepository.UpdateDisc(disc, cancellationToken);
		}

		public async Task SetDiscCoverImage(DiscImageModel coverImage, Stream imageContent, CancellationToken cancellationToken)
		{
			var disc = coverImage.Disc;
			var currentDisc = await discsRepository.GetDisc(disc.Id, cancellationToken);

			if (currentDisc.CoverImage != null)
			{
				await DeleteDiscCoverImage(currentDisc, cancellationToken);
				disc.DeleteImage(disc.CoverImage);
			}

			disc.AddImage(coverImage);
			await AddDiscCoverImage(coverImage, imageContent, cancellationToken);
		}

		private async Task DeleteDiscCoverImage(DiscModel disc, CancellationToken cancellationToken)
		{
			await storageRepository.DeleteDiscImage(disc.CoverImage, cancellationToken);
			await discsRepository.DeleteDiscImage(disc.CoverImage, cancellationToken);
		}

		private async Task AddDiscCoverImage(DiscImageModel coverImage, Stream imageContent, CancellationToken cancellationToken)
		{
			await storageRepository.AddDiscImage(coverImage, imageContent, cancellationToken);
			await discsRepository.AddDiscImage(coverImage, cancellationToken);
		}

		public async Task DeleteDisc(ItemId discId, string deleteComment, CancellationToken cancellationToken)
		{
			var deleteTime = clock.Now;

			var disc = await discsRepository.GetDisc(discId, cancellationToken);
			logger.LogInformation($"Deleting the disc '{disc.TreeTitle}' ...");

			foreach (var song in disc.ActiveSongs)
			{
				await songsService.DeleteSong(song, deleteTime, deleteComment, cancellationToken);
			}

			if (disc.CoverImage != null)
			{
				logger.LogInformation($"Deleting disc cover image '{disc.CoverImage.TreeTitle}' ...");
				await DeleteDiscCoverImage(disc, cancellationToken);
			}

			var updateDisc = false;
			var deleteOrphanAdviseGroups = false;
			if (disc.AdviseGroup != null)
			{
				// We erase advise group so that it could be deleted when no references left.
				disc.AdviseGroup = null;
				updateDisc = true;
				deleteOrphanAdviseGroups = true;
			}

			if (disc.AdviseSetInfo != null)
			{
				// We erase advise set for several reasons:
				// 1. In order to unnecessary (empty) advise sets could be deleted (manually).
				// 2. In order to deleted discs do not interfere with active discs within same advise set.
				//    Otherwise some care should be taken in adviser for handling this case.
				// Overall there is no any use of advise sets for deleted discs.
				disc.AdviseSetInfo = null;
				updateDisc = true;
			}

			if (updateDisc)
			{
				await discsRepository.UpdateDisc(disc, cancellationToken);
			}

			if (deleteOrphanAdviseGroups)
			{
				await adviseGroupRepository.DeleteOrphanAdviseGroups(cancellationToken);
			}

			logger.LogInformation($"The Disc '{disc.TreeTitle}' was deleted successfully");
		}
	}
}
