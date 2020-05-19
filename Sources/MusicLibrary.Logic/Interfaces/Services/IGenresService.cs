using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Services
{
	public interface IGenresService
	{
		Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken);
	}
}
