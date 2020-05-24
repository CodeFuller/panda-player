using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Interfaces.Services;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Services
{
	internal class ArtistsService : IArtistsService
	{
		private readonly IArtistsRepository artistsRepository;

		public ArtistsService(IArtistsRepository artistsRepository)
		{
			this.artistsRepository = artistsRepository ?? throw new ArgumentNullException(nameof(artistsRepository));
		}

		public Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken)
		{
			// TODO: We should return artists only from active songs (logic in service?)
			var artists = artistsRepository.GetAllArtists()
				.OrderBy(a => a.Name)
				.ToList();

			return Task.FromResult<IReadOnlyCollection<ArtistModel>>(artists);
		}
	}
}
