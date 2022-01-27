using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IGenresRepository
	{
		Task<IReadOnlyCollection<GenreModel>> GetEmptyGenres(CancellationToken cancellationToken);
	}
}
