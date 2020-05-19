using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
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
				.Select(g => new ArtistModel
				{
					Id = new ItemId(g.Id.ToString(CultureInfo.InvariantCulture)),
					Name = g.Name,
				})
				.ToList();

			return Task.FromResult<IReadOnlyCollection<ArtistModel>>(artists);
		}
	}
}
