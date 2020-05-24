using System;
using System.Globalization;
using System.Linq;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class ArtistsRepository : IArtistsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		public ArtistsRepository(IMusicLibraryDbContextFactory contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
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
