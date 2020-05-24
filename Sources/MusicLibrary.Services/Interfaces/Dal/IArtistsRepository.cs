using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		IQueryable<ArtistModel> GetAllArtists();
	}
}
