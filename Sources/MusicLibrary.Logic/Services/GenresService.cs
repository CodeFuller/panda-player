using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
{
	internal class GenresService : IGenresService
	{
		private readonly IGenresRepository genresRepository;

		public GenresService(IGenresRepository genresRepository)
		{
			this.genresRepository = genresRepository ?? throw new ArgumentNullException(nameof(genresRepository));
		}

		public async Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken)
		{
			var genres = await genresRepository.GetAllGenres(cancellationToken);

			return genres.OrderBy(g => g.Name)
				.ToList();
		}
	}
}
