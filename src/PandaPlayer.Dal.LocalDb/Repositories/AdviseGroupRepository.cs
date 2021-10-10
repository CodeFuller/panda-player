using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Extensions;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class AdviseGroupRepository : IAdviseGroupRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		private readonly ILogger<AdviseGroupRepository> logger;

		public AdviseGroupRepository(IDbContextFactory<MusicDbContext> contextFactory, ILogger<AdviseGroupRepository> logger)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CreateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			var adviseGroupEntity = adviseGroup.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.AdviseGroups.AddAsync(adviseGroupEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			adviseGroup.Id = adviseGroupEntity.Id.ToItemId();
		}

		public async Task<IReadOnlyCollection<AdviseGroupModel>> GetAllAdviseGroups(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			return (await context.AdviseGroups.ToListAsync(cancellationToken))
				.Select(a => a.ToModel())
				.ToList();
		}

		public async Task UpdateAdviseGroup(AdviseGroupModel adviseGroup, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var currentEntity = await context.AdviseGroups
				.SingleAsync(x => x.Id == adviseGroup.Id.ToInt32(), cancellationToken);

			var updatedEntity = adviseGroup.ToEntity();
			context.Entry(currentEntity).CurrentValues.SetValues(updatedEntity);

			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task DeleteOrphanAdviseGroups(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var orphanAdviseGroups = await context.AdviseGroups.Where(x => !x.Folders.Any() && !x.Discs.Any()).ToListAsync(cancellationToken);

			if (!orphanAdviseGroups.Any())
			{
				return;
			}

			foreach (var orphanAdviseGroup in orphanAdviseGroups)
			{
				logger.LogInformation($"Deleting advise group '{orphanAdviseGroup.Name}' ...");
			}

			context.AdviseGroups.RemoveRange(orphanAdviseGroups);
			await context.SaveChangesAsync(cancellationToken);
		}
	}
}
