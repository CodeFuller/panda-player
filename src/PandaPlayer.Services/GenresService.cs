using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;
using PandaPlayer.Services.Interfaces;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services
{
	internal class GenresService : IGenresService
	{
		private static IDiscLibrary DiscLibrary => DiscLibraryHolder.DiscLibrary;

		public Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken)
		{
			return Task.FromResult(DiscLibrary.Genres);
		}
	}
}
