using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class DiscsRepository : IDiscsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IMusicLibrary musicLibrary;

		private readonly IDataStorage dataStorage;

		public DiscsRepository(IMusicLibraryDbContextFactory contextFactory, IMusicLibrary musicLibrary, IDataStorage dataStorage)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
		}

		public async Task<IReadOnlyCollection<DiscModel>> GetDiscs(IEnumerable<ItemId> discIds, CancellationToken cancellationToken)
		{
			var ids = discIds.Select(id => id.ToInt32());

			await using var context = contextFactory.Create();

			return await context.Discs
				.Include(disc => disc.Songs).ThenInclude(song => song.Artist)
				.Include(disc => disc.Songs).ThenInclude(song => song.Genre)
				.Include(disc => disc.Songs).ThenInclude(song => song.Playbacks)
				.Include(disc => disc.Images)
				.Where(disc => ids.Contains(disc.Id))
				.Select(disc => disc.ToModel(dataStorage))
				.ToListAsync(cancellationToken);
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
