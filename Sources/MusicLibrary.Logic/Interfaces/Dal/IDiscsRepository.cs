using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Interfaces.Dal
{
	public interface IDiscsRepository
	{
		Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken);

		Task UpdateDisc(DiscModel discModel, CancellationToken cancellationToken);

		Task DeleteDisc(ItemId discId, CancellationToken cancellationToken);
	}
}
