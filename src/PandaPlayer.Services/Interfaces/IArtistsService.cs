using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IArtistsService
	{
		Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken);
	}
}
