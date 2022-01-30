using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IFoldersRepository
	{
		Task CreateEmptyFolder(FolderModel folder, CancellationToken cancellationToken);

		Task UpdateFolder(FolderModel folder, CancellationToken cancellationToken);
	}
}
