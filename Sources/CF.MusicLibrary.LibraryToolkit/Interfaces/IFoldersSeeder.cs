using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CF.MusicLibrary.Core.Objects;

namespace CF.MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IFoldersSeeder
	{
		Task<IReadOnlyDictionary<Uri, int>> SeedFolders(DiscLibrary discLibrary, CancellationToken cancellationToken);
	}
}
