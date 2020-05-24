using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		IQueryable<ArtistModel> GetAllArtists();
	}
}
