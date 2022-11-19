using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class DiscsService : IDiscsService
	{
		private readonly IDiscsRepository discsRepository;

		private readonly ISongsService songsService;

		private readonly ISongsRepository songsRepository;

		private readonly IStorageRepository storageRepository;

		private readonly IAdviseGroupService adviseGroupService;

		private readonly IClock clock;

		private readonly ILogger<DiscsService> logger;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public DiscsService(IDiscsRepository discsRepository, ISongsService songsService, ISongsRepository songsRepository,
			IStorageRepository storageRepository, IAdviseGroupService adviseGroupService, IClock clock, ILogger<DiscsService> logger)
		{
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.songsRepository = songsRepository ?? throw new ArgumentNullException(nameof(songsRepository));
			this.storageRepository = storageRepository ?? throw new ArgumentNullException(nameof(storageRepository));
			this.adviseGroupService = adviseGroupService ?? throw new ArgumentNullException(nameof(adviseGroupService));
			this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await storageRepository.CreateDisc(disc, cancellationToken);

			await discsRepository.CreateDisc(disc, cancellationToken);

			DiscLibrary.AddDisc(disc);
		}

		public Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.Discs);
		}

		public async Task UpdateDisc(DiscModel disc, Action<DiscModel> updateAction, CancellationToken cancellationToken)
		{
			var currentDisc = disc.CloneShallow();

			updateAction(disc);

			if (!disc.IsDeleted && disc.TreeTitle != currentDisc.TreeTitle)
			{
				await storageRepository.UpdateDiscTreeTitle(currentDisc, disc, cancellationToken);
			}

			if (disc.AlbumTitle != currentDisc.AlbumTitle || disc.Year != currentDisc.Year)
			{
				foreach (var song in disc.ActiveSongs)
				{
					await storageRepository.UpdateSong(song, cancellationToken);

					// Update in repository should be performed after update in the storage, because later updates song checksum.
					await songsRepository.UpdateSong(song, cancellationToken);
				}
			}

			await discsRepository.UpdateDisc(disc, cancellationToken);
		}

		public async Task SetDiscCoverImage(DiscModel disc, DiscImageModel coverImage, Stream imageContent, CancellationToken cancellationToken)
		{
			if (disc.CoverImage != null)
			{
				await DeleteDiscCoverImage(disc.CoverImage, cancellationToken);
			}

			disc.AddImage(coverImage);
			await AddDiscCoverImage(coverImage, imageContent, cancellationToken);
		}

		private async Task DeleteDiscCoverImage(DiscImageModel coverImage, CancellationToken cancellationToken)
		{
			await storageRepository.DeleteDiscImage(coverImage, cancellationToken);
			await discsRepository.DeleteDiscImage(coverImage, cancellationToken);

			DiscLibrary.DeleteDiscImage(coverImage);

			var disc = coverImage.Disc;
			disc.DeleteImage(coverImage);
		}

		private async Task AddDiscCoverImage(DiscImageModel coverImage, Stream imageContent, CancellationToken cancellationToken)
		{
			await storageRepository.AddDiscImage(coverImage, imageContent, cancellationToken);
			await discsRepository.AddDiscImage(coverImage, cancellationToken);

			DiscLibrary.AddDiscImage(coverImage);
		}

		public async Task DeleteDisc(ItemId discId, string deleteComment, CancellationToken cancellationToken)
		{
			var deleteTime = clock.Now;

			var disc = DiscLibrary.GetDisc(discId);

			logger.LogInformation($"Deleting the disc '{disc.TreeTitle}' ...");

			foreach (var song in disc.ActiveSongs)
			{
				await songsService.DeleteSong(song, deleteTime, deleteComment, cancellationToken);
			}

			if (disc.CoverImage != null)
			{
				logger.LogInformation($"Deleting disc cover image '{disc.CoverImage.TreeTitle}' ...");
				await DeleteDiscCoverImage(disc.CoverImage, cancellationToken);
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
				await adviseGroupService.DeleteOrphanAdviseGroups(cancellationToken);
			}

			logger.LogInformation($"The Disc '{disc.TreeTitle}' was deleted successfully");
		}
	}
}
