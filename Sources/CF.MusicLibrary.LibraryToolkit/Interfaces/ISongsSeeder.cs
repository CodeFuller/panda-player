using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface ISongsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedSongs(DiscLibrary discLibrary, IReadOnlyDictionary<int, int> discs,
			IReadOnlyDictionary<int, int> artists, IReadOnlyDictionary<int, int> genres, CancellationToken cancellationToken);
	}
}
