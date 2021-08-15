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
	internal class ArtistsService : IArtistsService
	{
		private readonly IArtistsRepository artistsRepository;

		public ArtistsService(IArtistsRepository artistsRepository)
		{
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
		}

		public Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
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
