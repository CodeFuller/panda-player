using System.Linq;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IGenresRepository
	{
		IQueryable<GenreModel> GetAllGenres();
	}
}
