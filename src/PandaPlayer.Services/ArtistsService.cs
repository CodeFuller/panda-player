using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
{
	internal class ArtistsService : IArtistsService
	{
		private readonly IArtistsRepository artistsRepository;

		private readonly ILogger<ArtistsService> logger;

		public ArtistsService(IArtistsRepository artistsRepository, ILogger<ArtistsService> logger)
		{
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
			logger.LogInformation($"Creating artist {artist.Name} ...");

			return artistsRepository.CreateArtist(artist, cancellationToken);
		}

		public async Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken)
		{
			return (await artistsRepository.GetAllArtists(cancellationToken))
				.OrderBy(a => a.Name)
				.ToList();
		}
	}
}
