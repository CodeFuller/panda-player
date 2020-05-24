using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Services
{
	public interface IArtistsService
	{
		Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken);
	}
}
