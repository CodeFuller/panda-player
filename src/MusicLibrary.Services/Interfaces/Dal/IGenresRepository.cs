using System.Linq;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IGenresRepository
	{
		IQueryable<GenreModel> GetAllGenres();
	}
}
