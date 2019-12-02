using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IGenresSeeder
	{
		Task<IDictionary<string, int>> SeedGenres(DiscLibrary discLibrary, CancellationToken cancellationToken);
	}
}
