using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Entities;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class AdviseSetRepository : IAdviseSetRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public AdviseSetRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			var adviseSetEntity = adviseSet.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.AdviseSets.AddAsync(adviseSetEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			adviseSet.Id = adviseSetEntity.Id.ToItemId();
		}

		public async Task<IReadOnlyCollection<AdviseSetModel>> GetAllAdviseSets(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var entities = await context.AdviseSets
				.Include(x => x.Discs)
				.ToListAsync(cancellationToken);

			return entities
				.Select(x => x.ToModel())
				.ToList();
		}

		public async Task UpdateAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var currentEntity = await FindAdviseSet(context, adviseSet.Id, cancellationToken);

			var updatedEntity = adviseSet.ToEntity();
			context.Entry(currentEntity).CurrentValues.SetValues(updatedEntity);

			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteAdviseSet(AdviseSetModel adviseSet, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var entity = await FindAdviseSet(context, adviseSet.Id, cancellationToken);
			context.AdviseSets.Remove(entity);

			await context.SaveChangesAsync(cancellationToken);
		}

		public static async Task<AdviseSetEntity> FindAdviseSet(MusicDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await context.AdviseSets
				.SingleAsync(x => x.Id == entityId, cancellationToken);
		}
	}
}
