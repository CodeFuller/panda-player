using System;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PandaPlayer.Core.Models;
using PandaPlayer.Dal.LocalDb.Internal;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Dal.LocalDb.Repositories
{
	internal class GenresRepository : IGenresRepository
	{
		private readonly IDbContextFactory<MusicDbContext> contextFactory;

		public GenresRepository(IDbContextFactory<MusicDbContext> contextFactory)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		}

		public IQueryable<GenreModel> GetAllGenres()
		{
			var context = contextFactory.CreateDbContext();

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
