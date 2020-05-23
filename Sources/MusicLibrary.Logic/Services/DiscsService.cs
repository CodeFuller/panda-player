using System;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Interfaces.Services;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Logic.Services
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
