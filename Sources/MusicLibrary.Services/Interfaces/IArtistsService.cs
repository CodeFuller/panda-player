using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IArtistsService
	{
		Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<ArtistModel>> GetAllArtists(CancellationToken cancellationToken);
	}
}
