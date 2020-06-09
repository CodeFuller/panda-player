using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.DiscAdder.Interfaces
{
	internal interface IFolderProvider
	{
		Task<FolderModel> GetFolder(IEnumerable<string> path, CancellationToken cancellationToken);
	}
}
