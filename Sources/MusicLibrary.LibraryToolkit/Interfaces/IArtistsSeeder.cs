using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IArtistsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedArtists(CancellationToken cancellationToken);
	}
}
