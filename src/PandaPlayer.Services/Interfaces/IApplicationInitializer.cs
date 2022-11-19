using System.Threading;
using System.Threading.Tasks;

namespace PandaPlayer.Services.Interfaces
{
	public interface IApplicationInitializer
	{
		Task Initialize(CancellationToken cancellationToken);
	}
}
