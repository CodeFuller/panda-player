using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IGenresSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedGenres(CancellationToken cancellationToken);
	}
}
