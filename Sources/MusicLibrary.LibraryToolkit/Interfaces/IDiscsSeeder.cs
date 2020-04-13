using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Objects;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IDiscsSeeder
	{
		Task<IReadOnlyDictionary<int, int>> SeedDiscs(DiscLibrary discLibrary, IReadOnlyDictionary<Uri, int> folders, CancellationToken cancellationToken);
	}
}
