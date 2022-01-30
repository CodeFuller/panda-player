using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		Task CreateArtist(ArtistModel artist, CancellationToken cancellationToken);
	}
}
