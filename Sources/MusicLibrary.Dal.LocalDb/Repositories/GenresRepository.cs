using System;
using System.Globalization;
using System.Linq;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Interfaces;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class GenresRepository : IGenresRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		public GenresRepository(IMusicLibraryDbContextFactory contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public IQueryable<GenreModel> GetAllGenres()
		{
			var context = contextFactory.Create();

			return context.Genres
				.Select(g => new GenreModel
				{
					Id = new ItemId(g.Id.ToString(CultureInfo.InvariantCulture)),
					Name = g.Name,
				})
				.AsQueryable();
		}
	}
}
