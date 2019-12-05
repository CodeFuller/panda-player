using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IDiscsSeeder
	{
		Task<IDictionary<Uri, int>> SeedDiscs(DiscLibrary discLibrary, IDictionary<Uri, int> folders, CancellationToken cancellationToken);
	}
}
