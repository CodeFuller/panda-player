using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces.Dal;
using MusicLibrary.Core.Interfaces.Services;
using MusicLibrary.Core.Models;

namespace MusicLibrary.Core.Services
{
	internal class DiscsService : IDiscsService
	{
		private readonly IDiscsRepository discsRepository;

		public DiscsService(IDiscsRepository discsRepository)
		{
			this.discsRepository = discsRepository ?? throw new ArgumentNullException(nameof(discsRepository));
		}

		public Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			return discsRepository.UpdateDisc(disc, cancellationToken);
		}

		public Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			return discsRepository.DeleteDisc(discId, cancellationToken);
		}
	}
}
