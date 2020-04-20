using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Dal.Abstractions.Dto;

namespace MusicLibrary.Dal.Abstractions.Interfaces
{
	public interface IDiscsRepository
	{
		Task DeleteDisc(ItemId discId, CancellationToken cancellationToken);
	}
}
