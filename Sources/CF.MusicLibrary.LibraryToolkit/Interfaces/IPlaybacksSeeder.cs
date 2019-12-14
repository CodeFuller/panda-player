using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IPlaybacksSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedPlaybacks(DiscLibrary discLibrary, IReadOnlyDictionary<int, int> songs, CancellationToken cancellationToken);
	}
}
