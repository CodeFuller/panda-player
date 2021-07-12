using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Interfaces.Dal;

namespace PandaPlayer.Services
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
			var artists = genresRepository.GetAllGenres()
				.OrderBy(a => a.Name)
				.ToList();

			return Task.FromResult<IReadOnlyCollection<GenreModel>>(artists);
		}
	}
}
