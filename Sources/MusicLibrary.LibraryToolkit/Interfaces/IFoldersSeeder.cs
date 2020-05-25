using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.LibraryToolkit.Interfaces
{
	public interface IFoldersSeeder
	{
		Task<IReadOnlyDictionary<Uri, int>> SeedFolders(CancellationToken cancellationToken);
	}
}
