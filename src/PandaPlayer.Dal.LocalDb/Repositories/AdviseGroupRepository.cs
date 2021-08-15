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
	}
}
