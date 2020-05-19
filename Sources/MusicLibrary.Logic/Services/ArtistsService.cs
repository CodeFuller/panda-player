using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
{
	internal class ArtistsService : IArtistsService
	{
		private readonly IArtistsRepository artistsRepository;

		public ArtistsService(IArtistsRepository artistsRepository)
		{
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
		}

		public async Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken)
		{
			var genres = await artistsRepository.GetAllArtists(cancellationToken);

			return genres.OrderBy(g => g.Name)
				.ToList();
		}
	}
}
