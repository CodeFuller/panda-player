using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IGenresRepository
	{
		Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken);
	}
}
