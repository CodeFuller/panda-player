using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces.Dal
{
	public interface IDiscsRepository
	{
		Task CreateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken);

		Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken);

		Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task AddDiscImage(DiscImageModel image, CancellationToken cancellationToken);

		Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken);
	}
}
