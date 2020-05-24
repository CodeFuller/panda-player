using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class DiscsRepository : IDiscsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IUriTranslator uriTranslator;

		public DiscsRepository(IMusicLibraryDbContextFactory contextFactory, IUriTranslator uriTranslator)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.uriTranslator = uriTranslator ?? throw new ArgumentNullException(nameof(uriTranslator));
		}

		public async Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			return (await GetDiscsQueryable(context)
					.ToListAsync(cancellationToken))
					.Select(disc => disc.ToModel(uriTranslator))
					.ToList();
		}

		public async Task<IReadOnlyCollection<DiscModel>> GetDiscs(IEnumerable<ItemId> discIds, CancellationToken cancellationToken)
		{
			var ids = discIds.Select(id => id.ToInt32());

			await using var context = contextFactory.Create();

			return await GetDiscsQueryable(context)
				.Where(disc => ids.Contains(disc.Id))
				.Select(disc => disc.ToModel(uriTranslator))
				.ToListAsync(cancellationToken);
		}

		private static IQueryable<DiscEntity> GetDiscsQueryable(MusicLibraryDbContext context)
		{
			return context.Discs
				.Include(disc => disc.Songs).ThenInclude(song => song.Artist)
				.Include(disc => disc.Songs).ThenInclude(song => song.Genre)
				.Include(disc => disc.Images);
		}

		public Task UpdateDisc(DiscModel discModel, CancellationToken cancellationToken)
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			// TODO: Implement
			throw new NotImplementedException();
		}
	}
}
