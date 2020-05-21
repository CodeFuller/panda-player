using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	internal class ArtistsRepository : IArtistsRepository
	{
		private readonly DiscLibrary discLibrary;

		public ArtistsRepository(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken)
		{
			var artists = discLibrary.Artists
				.Select(a => a.ToModel())
				.ToList();

			return Task.FromResult<IReadOnlyCollection<ArtistModel>>(artists);
		}
	}
}
