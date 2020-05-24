using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces
{
	public interface IDiscsService
	{
		Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task DeleteDisc(ItemId discId, CancellationToken cancellationToken);
	}
}
