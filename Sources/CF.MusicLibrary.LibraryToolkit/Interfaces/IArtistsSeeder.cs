using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IArtistsSeeder
	{
		Task<IDictionary<string, int>> SeedArtists(DiscLibrary discLibrary, CancellationToken cancellationToken);
	}
}
