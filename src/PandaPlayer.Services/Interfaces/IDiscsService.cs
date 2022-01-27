using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PandaPlayer.Core.Models;

namespace PandaPlayer.Services.Interfaces
{
	public interface IDiscsService
	{
		Task CreateDisc(DiscModel disc, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken);

		Task UpdateDisc(DiscModel disc, Action<DiscModel> updateAction, CancellationToken cancellationToken);

		Task SetDiscCoverImage(DiscImageModel coverImage, Stream imageContent, CancellationToken cancellationToken);

		Task DeleteDisc(ItemId discId, string deleteComment, CancellationToken cancellationToken);
	}
}
