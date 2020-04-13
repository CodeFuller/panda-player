using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IPlaybacksSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedPlaybacks(DiscLibrary discLibrary, IReadOnlyDictionary<int, int> songs, CancellationToken cancellationToken);
	}
}
