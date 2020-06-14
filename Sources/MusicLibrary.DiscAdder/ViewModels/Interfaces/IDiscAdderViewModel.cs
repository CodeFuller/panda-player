using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.DiscAdder.ViewModels.Interfaces
{
	public interface IDiscAdderViewModel
	{
		Task Load(CancellationToken cancellationToken);
	}
}
