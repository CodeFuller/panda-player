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
	internal class GenresRepository : IGenresRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public GenresRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task<IReadOnlyCollection<GenreModel>> GetEmptyGenres(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var genres = await context.Genres
				.Include(x => x.Songs)
				.Where(x => !x.Songs.Any())
				.ToListAsync(cancellationToken);

			return genres.Select(x => x.ToModel()).ToList();
		}
	}
}
