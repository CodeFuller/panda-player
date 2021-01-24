using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class ArtistsRepository : IArtistsRepository
	{
		private readonly IDbContextFactory<MusicLibraryDbContext> contextFactory;

		public ArtistsRepository(IDbContextFactory<MusicLibraryDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
			var artistEntity = artist.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.Artists.AddAsync(artistEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			artist.Id = artistEntity.Id.ToItemId();
		}

		public IQueryable<ArtistModel> GetAllArtists()
		{
			var context = contextFactory.CreateDbContext();

			return context.Artists
				.Select(a => new ArtistModel
				{
					Id = new ItemId(a.Id.ToString(CultureInfo.InvariantCulture)),
					Name = a.Name,
				})
				.AsQueryable();
		}
	}
}
