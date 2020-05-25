using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IPlaybacksSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedPlaybacks(IReadOnlyDictionary<int, int> songs, CancellationToken cancellationToken);
	}
}
