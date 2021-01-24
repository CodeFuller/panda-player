using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class ArtistsRepository : IArtistsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		public ArtistsRepository(IMusicLibraryDbContextFactory contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public async Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken)
		{
			var artistEntity = artist.ToEntity();

			await using var context = contextFactory.Create();
			await context.Artists.AddAsync(artistEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			artist.Id = artistEntity.Id.ToItemId();
		}

		public IQueryable<ArtistModel> GetAllArtists()
		{
			var context = contextFactory.Create();

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
