using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;
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
					Id = new ItemId(g.Id.ToString(CultureInfo.InvariantCulture)),
					Name = g.Name,
				})
				.ToList();

			return Task.FromResult<IReadOnlyCollection<GenreModel>>(genres);
		}
	}
}
