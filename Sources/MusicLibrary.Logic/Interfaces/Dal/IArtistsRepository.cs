using System.Linq;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IArtistsRepository
	{
		IQueryable<ArtistModel> GetAllArtists();
	}
}
