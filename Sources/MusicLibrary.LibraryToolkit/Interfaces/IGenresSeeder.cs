using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IGenresSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedGenres(DiscLibrary discLibrary, CancellationToken cancellationToken);
	}
}
