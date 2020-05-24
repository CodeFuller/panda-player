using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Interfaces.Dal
{
	public interface IGenresRepository
	{
		IQueryable<GenreModel> GetAllGenres();
	}
}
