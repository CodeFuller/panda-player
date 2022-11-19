using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.DiscAdder.Interfaces
{
	internal interface IFolderProvider
	{
		Task<FolderModel> GetFolder(IEnumerable<string> path, CancellationToken cancellationToken);
	}
}
