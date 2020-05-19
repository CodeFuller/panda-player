using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken);
	}
}
