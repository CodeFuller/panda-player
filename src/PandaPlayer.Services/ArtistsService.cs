using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class ArtistsService : IArtistsService
	{
		private readonly IArtistsRepository artistsRepository;

		private readonly ILogger<ArtistsService> logger;

		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public ArtistsService(IArtistsRepository artistsRepository, ILogger<ArtistsService> logger)
		{
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
			logger.LogInformation($"Creating artist '{artist.Name}' ...");

			await artistsRepository.CreateArtist(artist, cancellationToken);

			DiscLibrary.AddArtist(artist);
		}

		public Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.Artists);
		}
	}
}
