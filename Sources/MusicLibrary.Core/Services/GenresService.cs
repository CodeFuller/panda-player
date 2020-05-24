using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Interfaces.Services;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Services
{
	internal class GenresService : IGenresService
	{
		private readonly IGenresRepository genresRepository;

		public GenresService(IGenresRepository genresRepository)
		{
			this.genresRepository = genresRepository ?? throw new ArgumentNullException(nameof(genresRepository));
		}

		public Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken)
		{
			// TODO: We should return genres only from active songs (logic in service?)
			var artists = genresRepository.GetAllGenres()
				.OrderBy(a => a.Name)
				.ToList();

			return Task.FromResult<IReadOnlyCollection<GenreModel>>(artists);
		}
	}
}
