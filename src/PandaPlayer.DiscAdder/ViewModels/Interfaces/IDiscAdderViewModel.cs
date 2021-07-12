using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.DiscAdder.ViewModels.Interfaces
{
	public interface IDiscAdderViewModel
	{
		Task Load(CancellationToken cancellationToken);
	}
}
