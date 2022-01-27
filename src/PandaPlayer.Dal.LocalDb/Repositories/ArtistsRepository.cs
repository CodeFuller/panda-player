using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class ArtistsRepository : IArtistsRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public ArtistsRepository(IDbContextFactory<MusicDbContext> contextFactory)
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

		public async Task<IEnumerable<ArtistModel>> GetEmptyArtists(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var artists = await context.Artists
				.Include(x => x.Songs)
				.Where(x => !x.Songs.Any())
				.ToListAsync(cancellationToken);

			return artists.Select(x => x.ToModel()).ToList();
		}
	}
}
