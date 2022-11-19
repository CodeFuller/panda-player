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
	internal class DiscsRepository : IDiscsRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public DiscsRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			var discEntity = disc.ToEntity();

			await using var context = contextFactory.CreateDbContext();
			await context.Discs.AddAsync(discEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			disc.Id = discEntity.Id.ToItemId();
		}

		public async Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var discEntity = await context.Discs
				.SingleAsync(d => d.Id == disc.Id.ToInt32(), cancellationToken);

			var updatedEntity = disc.ToEntity();
			context.Entry(discEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task AddDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var discEntity = await context.Discs
				.Include(x => x.Images)
				.SingleAsync(d => d.Id == image.Disc.Id.ToInt32(), cancellationToken);

			var imageEntity = image.ToEntity();
			discEntity.Images.Add(imageEntity);

			await context.SaveChangesAsync(cancellationToken);

			image.Id = imageEntity.Id.ToItemId();
		}

		public async Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.CreateDbContext();

			var imageEntity = await context.DiscImages.FindAsync(new object[] { image.Id.ToInt32() }, cancellationToken);
			context.DiscImages.Remove(imageEntity);

			await context.SaveChangesAsync(cancellationToken);
		}
	}
}
