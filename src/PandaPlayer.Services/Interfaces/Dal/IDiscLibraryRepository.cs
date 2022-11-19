using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Services.Internal;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IDiscLibraryRepository
	{
		Task<DiscLibrary> ReadDiscLibrary(CancellationToken cancellationToken);

		Task<DiscLibrary> ReadDiscLibraryWithPlaybacks(CancellationToken cancellationToken);
	}
}
