using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IGenresService
	{
		Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken);
	}
}
