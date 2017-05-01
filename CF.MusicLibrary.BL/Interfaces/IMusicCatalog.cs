using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicCatalog
	{
		Task<DiscLibrary> GetDiscsAsync();

		Task<IEnumerable<Genre>> GetAllGenresAsync();
	}
}
