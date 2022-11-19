using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.ViewModels.Interfaces
{
	public interface IRenameFolderViewModel
	{
		string FolderName { get; set; }

		void Load(FolderModel folder);

		Task Rename(CancellationToken cancellationToken);
	}
}
