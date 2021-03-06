﻿using System;
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

		private readonly IClock clock;

		private readonly ILogger<DiscsService> logger;

		public DiscsService(IDiscsRepository discsRepository, ISongsService songsService, IStorageRepository storageRepository, IClock clock, ILogger<DiscsService> logger)
		{
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
			this.songsService = songsService ?? throw new ArgumentNullException(nameof(songsService));
			this.storageRepository = storageRepository;
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

		public async Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			var deleteTime = clock.Now;

			var disc = await discsRepository.GetDisc(discId, cancellationToken);
			logger.LogInformation($"Deleting the disc '{disc.TreeTitle}' ...");

			foreach (var song in disc.ActiveSongs)
			{
				await songsService.DeleteSong(song, deleteTime, cancellationToken);
			}

			if (disc.CoverImage != null)
			{
				logger.LogInformation($"Deleting disc cover image '{disc.CoverImage.TreeTitle}' ...");
				await DeleteDiscCoverImage(disc, cancellationToken);
			}

			logger.LogInformation($"The Disc '{disc.TreeTitle}' was deleted successfully");
		}
	}
}
