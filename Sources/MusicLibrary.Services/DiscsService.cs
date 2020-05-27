using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Services
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

		public Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken)
		{
			return discsRepository.GetAllDiscs(cancellationToken);
		}

		public Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			return discsRepository.UpdateDisc(disc, cancellationToken);
		}

		public async Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			var deleteTime = clock.Now;

			var disc = await discsRepository.GetDisc(discId, cancellationToken);
			logger.LogInformation($"Deleting disc '{disc.TreeTitle}' ...");

			foreach (var song in disc.ActiveSongs)
			{
				await songsService.DeleteSong(song, deleteTime, cancellationToken);
			}

			foreach (var image in disc.Images)
			{
				logger.LogInformation($"Deleting disc image '{image.TreeTitle}' ...");
				await storageRepository.DeleteDiscImage(image, cancellationToken);
			}

			disc.Images = new List<DiscImageModel>();
			await discsRepository.UpdateDisc(disc, cancellationToken);

			logger.LogInformation($"Disc '{disc.TreeTitle}' was deleted successfully");
		}
	}
}
