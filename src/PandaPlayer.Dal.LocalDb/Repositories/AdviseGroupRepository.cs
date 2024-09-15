using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class AdviseGroupRepository : IAdviseGroupRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public AdviseGroupRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			var adviseGroupEntity = adviseGroup.ToEntity();

			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
			await context.AdviseGroups.AddAsync(adviseGroupEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			adviseGroup.Id = adviseGroupEntity.Id.ToItemId();
		}

		public async Task UpdateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var currentEntity = await context.AdviseGroups
				.SingleAsync(x => x.Id == adviseGroup.Id.ToInt32(), cancellationToken);

			var updatedEntity = adviseGroup.ToEntity();
			context.Entry(currentEntity).CurrentValues.SetValues(updatedEntity);

			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

			var currentEntity = await context.AdviseGroups
				.SingleAsync(x => x.Id == adviseGroup.Id.ToInt32(), cancellationToken);

			context.AdviseGroups.Remove(currentEntity);
			await context.SaveChangesAsync(cancellationToken);
		}
	}
}
