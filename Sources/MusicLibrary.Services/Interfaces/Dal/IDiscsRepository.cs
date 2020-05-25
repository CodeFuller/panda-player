using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Services.Interfaces.Dal
{
	public interface IDiscsRepository
	{
		Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken);

		Task<IReadOnlyCollection<DiscModel>> GetDiscs(IEnumerable<ItemId> discIds, CancellationToken cancellationToken);

		Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken);

		Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken);
	}
}
