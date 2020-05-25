using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface ISongsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedSongs(IReadOnlyDictionary<int, int> discs, IReadOnlyDictionary<int, int> artists,
			IReadOnlyDictionary<int, int> genres, CancellationToken cancellationToken);
	}
}
