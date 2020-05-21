using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	internal class GenresRepository : IGenresRepository
	{
		private readonly DiscLibrary discLibrary;

		public GenresRepository(DiscLibrary discLibrary)
		{
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken)
		{
			var genres = discLibrary.Genres
				.Select(g => new GenreModel
				{
					Id = g.Id.ToItemId(),
					Name = g.Name,
				})
				.ToList();

			return Task.FromResult<IReadOnlyCollection<GenreModel>>(genres);
		}
	}
}
