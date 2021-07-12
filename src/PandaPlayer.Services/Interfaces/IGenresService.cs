using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IGenresService
	{
		Task<IReadOnlyCollection<GenreModel>> GetAllGenres(CancellationToken cancellationToken);
	}
}
