using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IDiscsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedDiscs(IReadOnlyDictionary<Uri, int> folders, CancellationToken cancellationToken);
	}
}
