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

		public async Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken)
		{
			return (await genresRepository.GetAllGenres(cancellationToken))
				.OrderBy(g => g.Name)
				.ToList();
		}
	}
}
