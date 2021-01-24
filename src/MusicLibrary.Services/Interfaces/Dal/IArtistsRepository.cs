using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken);

		IQueryable<ArtistModel> GetAllArtists();
	}
}
