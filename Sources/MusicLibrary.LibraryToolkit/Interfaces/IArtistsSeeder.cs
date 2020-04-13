using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IArtistsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedArtists(DiscLibrary discLibrary, CancellationToken cancellationToken);
	}
}
